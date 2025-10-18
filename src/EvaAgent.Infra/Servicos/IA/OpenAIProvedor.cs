using System.Net.Http.Json;
using System.Text.Json.Serialization;
using EvaAgent.Dominio.Entidades.IA;
using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.IA;

public class OpenAIProvedor : IProvedorIA
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelo;
    private readonly int? _maxTokens;
    private readonly decimal? _temperatura;
    private readonly ILogger<OpenAIProvedor>? _logger;

    public string Nome => "OpenAI";

    public OpenAIProvedor(
        HttpClient httpClient,
        ProvedorIA config,
        ILogger<OpenAIProvedor>? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiKey = config.ChaveApi ?? throw new ArgumentNullException(nameof(config.ChaveApi));
        _modelo = config.Modelo ?? "gpt-4";
        _maxTokens = config.MaxTokens;
        _temperatura = config.Temperatura;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(config.UrlBase ?? "https://api.openai.com/v1/");
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
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

            if (!string.IsNullOrEmpty(contexto))
            {
                messages.Add(new { role = "system", content = contexto });
            }

            messages.Add(new { role = "user", content = prompt });

            var request = new
            {
                model = _modelo,
                messages,
                max_tokens = _maxTokens,
                temperature = _temperatura
            };

            _logger?.LogInformation(
                "Enviando solicitação para OpenAI - Modelo: {Modelo}, Tokens: {MaxTokens}",
                _modelo,
                _maxTokens);

            var response = await _httpClient.PostAsJsonAsync(
                "chat/completions",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger?.LogError(
                    "Erro na API OpenAI: {StatusCode} - {Error}",
                    response.StatusCode,
                    errorContent);
                throw new HttpRequestException(
                    $"Erro na API OpenAI: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>(cancellationToken);

            if (result == null || result.Choices == null || result.Choices.Count == 0)
            {
                throw new InvalidOperationException("Resposta inválida da API OpenAI");
            }

            var resposta = result.Choices[0].Message.Content;
            var tokensPrompt = result.Usage?.PromptTokens ?? 0;
            var tokensResposta = result.Usage?.CompletionTokens ?? 0;

            // Cálculo de custo baseado no modelo
            var custo = CalcularCusto(_modelo, tokensPrompt, tokensResposta);

            _logger?.LogInformation(
                "Resposta recebida da OpenAI - Tokens Input: {Input}, Output: {Output}, Custo: {Custo:C}",
                tokensPrompt,
                tokensResposta,
                custo);

            return (resposta, tokensPrompt, tokensResposta, custo);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro ao gerar resposta com OpenAI");
            throw;
        }
    }

    private static decimal CalcularCusto(string modelo, int tokensPrompt, int tokensResposta)
    {
        // Preços por 1K tokens (valores aproximados - verificar pricing atual)
        var (custoInput, custoOutput) = modelo.ToLowerInvariant() switch
        {
            var m when m.Contains("gpt-4-turbo") => (0.01m, 0.03m),
            var m when m.Contains("gpt-4") => (0.03m, 0.06m),
            var m when m.Contains("gpt-3.5-turbo") => (0.0015m, 0.002m),
            _ => (0.03m, 0.06m) // Default para GPT-4
        };

        return (tokensPrompt * custoInput / 1000) + (tokensResposta * custoOutput / 1000);
    }

    #region DTOs OpenAI

    private class OpenAIResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; } = string.Empty;

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; } = new();

        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }

    private class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; } = new();

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = string.Empty;
    }

    private class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    #endregion
}
