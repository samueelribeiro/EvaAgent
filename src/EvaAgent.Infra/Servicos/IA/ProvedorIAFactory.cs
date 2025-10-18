using EvaAgent.Dominio.Entidades.IA;
using EvaAgent.Dominio.Enums;
using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.IA;

/// <summary>
/// Factory para criar instâncias de provedores de IA
/// </summary>
public class ProvedorIAFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILoggerFactory? _loggerFactory;

    public ProvedorIAFactory(
        IHttpClientFactory httpClientFactory,
        ILoggerFactory? loggerFactory = null)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Cria uma instância do provedor de IA baseado na configuração
    /// </summary>
    public IProvedorIA Criar(ProvedorIA config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        if (!config.Habilitado)
            throw new InvalidOperationException($"Provedor '{config.Nome}' está desabilitado");

        if (string.IsNullOrEmpty(config.ChaveApi))
            throw new ArgumentException("ChaveApi é obrigatória", nameof(config));

        var httpClient = _httpClientFactory.CreateClient($"ProvedorIA_{config.Tipo}");

        return config.Tipo switch
        {
            TipoProvedor.OpenAI => new OpenAIProvedor(
                httpClient,
                config,
                _loggerFactory?.CreateLogger<OpenAIProvedor>()),

            TipoProvedor.Anthropic => new ClaudeProvedor(
                httpClient,
                config,
                _loggerFactory?.CreateLogger<ClaudeProvedor>()),

            TipoProvedor.Google => new GeminiProvedor(
                httpClient,
                config,
                _loggerFactory?.CreateLogger<GeminiProvedor>()),

            _ => throw new NotSupportedException(
                $"Tipo de provedor '{config.Tipo}' não é suportado")
        };
    }

    /// <summary>
    /// Cria uma instância do provedor de IA pelo tipo
    /// </summary>
    public IProvedorIA CriarPorTipo(
        TipoProvedor tipo,
        string chaveApi,
        string? modelo = null,
        int? maxTokens = null,
        decimal? temperatura = null,
        string? urlBase = null)
    {
        var config = new ProvedorIA
        {
            Tipo = tipo,
            ChaveApi = chaveApi,
            Modelo = modelo,
            MaxTokens = maxTokens,
            Temperatura = temperatura,
            UrlBase = urlBase,
            Habilitado = true
        };

        return Criar(config);
    }
}
