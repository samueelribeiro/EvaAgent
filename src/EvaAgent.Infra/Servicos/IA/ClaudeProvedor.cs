using System.Net.Http.Json;
using System.Text.Json.Serialization;
using EvaAgent.Dominio.Entidades.IA;
using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.IA;

public class ClaudeProvedor : IProvedorIA
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelo;
    private readonly int? _maxTokens;
    private readonly decimal? _temperatura;
    private readonly ILogger<ClaudeProvedor>? _logger;

    public string Nome => "Claude (Anthropic)";

    public ClaudeProvedor(
        HttpClient httpClient,
        ProvedorIA config,
        ILogger<ClaudeProvedor>? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiKey = config.ChaveApi ?? throw new ArgumentNullException(nameof(config.ChaveApi));
        _modelo = config.Modelo ?? "claude-3-opus-20240229";
        _maxTokens = config.MaxTokens ?? 4096;
        _temperatura = config.Temperatura;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(config.UrlBase ?? "https://api.anthropic.com/v1/");
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
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
            var messages = new List<object>();

            // Claude usa o campo "system" separado dos messages
            var systemPrompt = contexto;

            messages.Add(new { role = "user", content = prompt });

            var request = new
            {
                model = _modelo,
                max_tokens = _maxTokens,
                temperature = _temperatura,
                system = systemPrompt,
                messages
            };

            _logger?.LogInformation(
                "Enviando solicitação para Claude - Modelo: {Modelo}, Tokens: {MaxTokens}",
                _modelo,
                _maxTokens);

            var response = await _httpClient.PostAsJsonAsync(
                "messages",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger?.LogError(
                    "Erro na API Claude: {StatusCode} - {Error}",
                    response.StatusCode,
                    errorContent);
                throw new HttpRequestException(
                    $"Erro na API Claude: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<ClaudeResponse>(cancellationToken);

            if (result == null || result.Content == null || result.Content.Count == 0)
            {
                throw new InvalidOperationException("Resposta inválida da API Claude");
            }

            var resposta = result.Content[0].Text;
            var tokensPrompt = result.Usage?.InputTokens ?? 0;
            var tokensResposta = result.Usage?.OutputTokens ?? 0;

            // Cálculo de custo baseado no modelo
            var custo = CalcularCusto(_modelo, tokensPrompt, tokensResposta);

            _logger?.LogInformation(
                "Resposta recebida do Claude - Tokens Input: {Input}, Output: {Output}, Custo: {Custo:C}",
                tokensPrompt,
                tokensResposta,
                custo);

            return (resposta, tokensPrompt, tokensResposta, custo);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao gerar resposta com Claude");
            throw;
        }
    }

    private static decimal CalcularCusto(string modelo, int tokensPrompt, int tokensResposta)
    {
        // Preços por 1M tokens (valores aproximados - verificar pricing atual)
        var (custoInput, custoOutput) = modelo.ToLowerInvariant() switch
        {
            var m when m.Contains("claude-3-opus") => (15.00m, 75.00m),
            var m when m.Contains("claude-3-sonnet") => (3.00m, 15.00m),
            var m when m.Contains("claude-3-haiku") => (0.25m, 1.25m),
            _ => (15.00m, 75.00m) // Default para Opus
        };

        return (tokensPrompt * custoInput / 1_000_000) + (tokensResposta * custoOutput / 1_000_000);
    }

    #region DTOs Claude

    private class ClaudeResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public List<ContentBlock> Content { get; set; } = new();

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("stop_reason")]
        public string? StopReason { get; set; }

        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }

    private class ContentBlock
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    private class Usage
    {
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; set; }

        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; set; }
    }

    #endregion
}
