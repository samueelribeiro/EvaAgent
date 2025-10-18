using System.Net;
using System.Text.Json;
using EvaAgent.Dominio.Entidades.IA;
using EvaAgent.Dominio.Enums;
using EvaAgent.Infra.Servicos.IA;
using Xunit;

namespace EvaAgent.Infra.Tests.Servicos.IA;

public class OpenAIProvedorTests
{
    [Fact]
    public async Task GerarResposta_DeveRetornarTextoValido()
    {
        // Arrange
        var mockResponse = new
        {
            id = "chatcmpl-123",
            @object = "chat.completion",
            created = 1677652288,
            model = "gpt-4",
            choices = new[]
            {
                new
                {
                    index = 0,
                    message = new
                    {
                        role = "assistant",
                        content = "Esta é uma resposta de teste do GPT-4."
                    },
                    finish_reason = "stop"
                }
            },
            usage = new
            {
                prompt_tokens = 10,
                completion_tokens = 20,
                total_tokens = 30
            }
        };

        var httpClient = CriarHttpClientMock(mockResponse);

        var config = new ProvedorIA
        {
            Tipo = TipoProvedor.OpenAI,
            ChaveApi = "sk-test-key",
            Modelo = "gpt-4",
            MaxTokens = 2000,
            Temperatura = 0.7m,
            Habilitado = true
        };

        var provedor = new OpenAIProvedor(httpClient, config);

        // Act
        var resposta = await provedor.GerarRespostaAsync(
            "Olá, como você está?",
            "Você é um assistente útil.");

        // Assert
        Assert.NotNull(resposta);
        Assert.Equal("Esta é uma resposta de teste do GPT-4.", resposta);
    }

    [Fact]
    public async Task GerarRespostaDetalhada_DeveRetornarTokensECusto()
    {
        // Arrange
        var mockResponse = new
        {
            id = "chatcmpl-123",
            @object = "chat.completion",
            created = 1677652288,
            model = "gpt-4",
            choices = new[]
            {
                new
                {
                    index = 0,
                    message = new
                    {
                        role = "assistant",
                        content = "Resposta detalhada."
                    },
                    finish_reason = "stop"
                }
            },
            usage = new
            {
                prompt_tokens = 100,
                completion_tokens = 50,
                total_tokens = 150
            }
        };

        var httpClient = CriarHttpClientMock(mockResponse);

        var config = new ProvedorIA
        {
            Tipo = TipoProvedor.OpenAI,
            ChaveApi = "sk-test-key",
            Modelo = "gpt-4",
            Habilitado = true
        };

        var provedor = new OpenAIProvedor(httpClient, config);

        // Act
        var (resposta, tokensPrompt, tokensResposta, custo) =
            await provedor.GerarRespostaDetalhadaAsync("Teste");

        // Assert
        Assert.Equal("Resposta detalhada.", resposta);
        Assert.Equal(100, tokensPrompt);
        Assert.Equal(50, tokensResposta);
        Assert.True(custo > 0, "Custo deve ser maior que zero");
    }

    [Fact]
    public async Task GerarResposta_ComChaveInvalida_DeveLancarExcecao()
    {
        // Arrange
        var httpClient = CriarHttpClientMockComErro(
            HttpStatusCode.Unauthorized,
            "Invalid API key");

        var config = new ProvedorIA
        {
            Tipo = TipoProvedor.OpenAI,
            ChaveApi = "sk-invalid-key",
            Modelo = "gpt-4",
            Habilitado = true
        };

        var provedor = new OpenAIProvedor(httpClient, config);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => provedor.GerarRespostaAsync("Teste"));
    }

    private static HttpClient CriarHttpClientMock(object responseObject)
    {
        var handler = new MockHttpMessageHandler(responseObject);
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.openai.com/v1/")
        };
    }

    private static HttpClient CriarHttpClientMockComErro(
        HttpStatusCode statusCode,
        string errorMessage)
    {
        var handler = new MockHttpMessageHandler(statusCode, errorMessage);
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.openai.com/v1/")
        };
    }
}

/// <summary>
/// Mock handler para testes HTTP
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly object? _responseObject;
    private readonly HttpStatusCode _statusCode;
    private readonly string? _errorMessage;

    public MockHttpMessageHandler(object responseObject)
    {
        _responseObject = responseObject;
        _statusCode = HttpStatusCode.OK;
    }

    public MockHttpMessageHandler(HttpStatusCode statusCode, string errorMessage)
    {
        _statusCode = statusCode;
        _errorMessage = errorMessage;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_statusCode);

        if (_statusCode == HttpStatusCode.OK && _responseObject != null)
        {
            var json = JsonSerializer.Serialize(_responseObject);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }
        else if (!string.IsNullOrEmpty(_errorMessage))
        {
            response.Content = new StringContent(_errorMessage);
        }

        return Task.FromResult(response);
    }
}
