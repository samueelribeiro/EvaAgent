using System.Net.Http.Json;
using System.Text.Json.Serialization;
using EvaAgent.Dominio.Entidades.IA;
using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.IA;

public class GeminiProvedor : IProvedorIA
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelo;
    private readonly int? _maxTokens;
    private readonly decimal? _temperatura;
    private readonly ILogger<GeminiProvedor>? _logger;

    public string Nome => "Gemini (Google)";

    public GeminiProvedor(
        HttpClient httpClient,
        ProvedorIA config,
        ILogger<GeminiProvedor>? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiKey = config.ChaveApi ?? throw new ArgumentNullException(nameof(config.ChaveApi));
        _modelo = config.Modelo ?? "gemini-pro";
        _maxTokens = config.MaxTokens;
        _temperatura = config.Temperatura;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(
            config.UrlBase ?? "https://generativelanguage.googleapis.com/v1beta/");
    }

    public async Task<string> GerarRespostaAsync(
        string prompt,
        string? contexto = null,
        CancellationToken cancellationToken = default)
    {
        var (resposta, _, _, _) = await GerarRespostaDetalhadaAsync(prompt, contexto, cancellationToken);
        return resposta;
    }

    public async Task<(string Resposta, int TokensPrompt, int TokensResposta, decimal CustoEstimado)>
        GerarRespostaDetalhadaAsync(
            string prompt,
            string? contexto = null,
            CancellationToken cancellationToken = default)
    {
        try
        {
            var contents = new List<object>();

            // Gemini combina system e user em uma única mensagem
            var textoCompleto = string.IsNullOrEmpty(contexto)
                ? prompt
                : $"{contexto}\n\n{prompt}";

            contents.Add(new
            {
                parts = new[] { new { text = textoCompleto } }
            });

            var request = new
            {
                contents,
                generationConfig = new
                {
                    temperature = _temperatura,
                    maxOutputTokens = _maxTokens,
                    topK = 40,
                    topP = 0.95
                }
            };

            _logger?.LogInformation(
                "Enviando solicitação para Gemini - Modelo: {Modelo}, Tokens: {MaxTokens}",
                _modelo,
                _maxTokens);

            var url = $"models/{_modelo}:generateContent?key={_apiKey}";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger?.LogError(
                    "Erro na API Gemini: {StatusCode} - {Error}",
                    response.StatusCode,
                    errorContent);
                throw new HttpRequestException(
                    $"Erro na API Gemini: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken);

            if (result == null || result.Candidates == null || result.Candidates.Count == 0)
            {
                throw new InvalidOperationException("Resposta inválida da API Gemini");
            }

            var candidate = result.Candidates[0];
            if (candidate.Content?.Parts == null || candidate.Content.Parts.Count == 0)
            {
                throw new InvalidOperationException("Conteúdo vazio na resposta do Gemini");
            }

            var resposta = candidate.Content.Parts[0].Text;
            var tokensPrompt = result.UsageMetadata?.PromptTokenCount ?? 0;
            var tokensResposta = result.UsageMetadata?.CandidatesTokenCount ?? 0;

            // Cálculo de custo baseado no modelo
            var custo = CalcularCusto(_modelo, tokensPrompt, tokensResposta);

            _logger?.LogInformation(
                "Resposta recebida do Gemini - Tokens Input: {Input}, Output: {Output}, Custo: {Custo:C}",
                tokensPrompt,
                tokensResposta,
                custo);

            return (resposta, tokensPrompt, tokensResposta, custo);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao gerar resposta com Gemini");
            throw;
        }
    }

    private static decimal CalcularCusto(string modelo, int tokensPrompt, int tokensResposta)
    {
        // Preços por 1M tokens (valores aproximados - verificar pricing atual)
        var (custoInput, custoOutput) = modelo.ToLowerInvariant() switch
        {
            var m when m.Contains("gemini-1.5-pro") => (3.50m, 10.50m),
            var m when m.Contains("gemini-pro") => (0.50m, 1.50m),
            var m when m.Contains("gemini-1.5-flash") => (0.35m, 1.05m),
            _ => (0.50m, 1.50m) // Default para Gemini Pro
        };

        return (tokensPrompt * custoInput / 1_000_000) + (tokensResposta * custoOutput / 1_000_000);
    }

    #region DTOs Gemini

    private class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; } = new();

        [JsonPropertyName("usageMetadata")]
        public UsageMetadata? UsageMetadata { get; set; }
    }

    private class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }

    private class Content
    {
        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; } = new();

        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    private class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    private class UsageMetadata
    {
        [JsonPropertyName("promptTokenCount")]
        public int PromptTokenCount { get; set; }

        [JsonPropertyName("candidatesTokenCount")]
        public int CandidatesTokenCount { get; set; }

        [JsonPropertyName("totalTokenCount")]
        public int TotalTokenCount { get; set; }
    }

    #endregion
}
