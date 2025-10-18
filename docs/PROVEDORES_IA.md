# Provedores de IA - Guia Completo

## 📋 Visão Geral

O sistema suporta múltiplos provedores de IA (LLM) de forma transparente através do padrão **Strategy Pattern**. Todos os provedores implementam a interface `IProvedorIA`.

## 🤖 Provedores Implementados

### 1. OpenAI (GPT-4, GPT-3.5)

**Arquivo**: `src/EvaAgent.Infra/Servicos/IA/OpenAIProvedor.cs`

#### Modelos Suportados
- `gpt-4-turbo` - Mais recente, rápido e eficiente
- `gpt-4` - Modelo principal (padrão)
- `gpt-3.5-turbo` - Mais rápido e econômico

#### Configuração
```csharp
var config = new ProvedorIA
{
    Tipo = TipoProvedor.OpenAI,
    Nome = "OpenAI GPT-4",
    ChaveApi = "sk-...",  // Sua chave da OpenAI
    Modelo = "gpt-4",
    MaxTokens = 2000,
    Temperatura = 0.7m,
    UrlBase = "https://api.openai.com/v1/",  // Opcional
    Habilitado = true
};
```

#### Custos (por 1K tokens)
| Modelo | Input | Output |
|--------|-------|--------|
| GPT-4 Turbo | $0.01 | $0.03 |
| GPT-4 | $0.03 | $0.06 |
| GPT-3.5 Turbo | $0.0015 | $0.002 |

#### Exemplo de Uso
```csharp
var provedor = new OpenAIProvedor(httpClient, config, logger);

var resposta = await provedor.GerarRespostaAsync(
    prompt: "Qual foi o valor das vendas de hoje?",
    contexto: "Você é um assistente financeiro especializado."
);

// Ou com detalhes
var (texto, tokensIn, tokensOut, custo) =
    await provedor.GerarRespostaDetalhadaAsync(prompt, contexto);
```

### 2. Claude (Anthropic)

**Arquivo**: `src/EvaAgent.Infra/Servicos/IA/ClaudeProvedor.cs`

#### Modelos Suportados
- `claude-3-opus-20240229` - Mais poderoso (padrão)
- `claude-3-sonnet-20240229` - Balanceado
- `claude-3-haiku-20240307` - Mais rápido e econômico

#### Configuração
```csharp
var config = new ProvedorIA
{
    Tipo = TipoProvedor.Anthropic,
    Nome = "Claude 3 Opus",
    ChaveApi = "sk-ant-...",  // Sua chave da Anthropic
    Modelo = "claude-3-opus-20240229",
    MaxTokens = 4096,
    Temperatura = 0.7m,
    UrlBase = "https://api.anthropic.com/v1/",  // Opcional
    Habilitado = true
};
```

#### Custos (por 1M tokens)
| Modelo | Input | Output |
|--------|-------|--------|
| Opus | $15.00 | $75.00 |
| Sonnet | $3.00 | $15.00 |
| Haiku | $0.25 | $1.25 |

#### Diferenças Importantes
- Claude usa header `x-api-key` (não Bearer)
- Requer header `anthropic-version: 2023-06-01`
- System prompt é campo separado (não faz parte das messages)

### 3. Gemini (Google)

**Arquivo**: `src/EvaAgent.Infra/Servicos/IA/GeminiProvedor.cs`

#### Modelos Suportados
- `gemini-1.5-pro` - Mais avançado
- `gemini-pro` - Modelo principal (padrão)
- `gemini-1.5-flash` - Mais rápido

#### Configuração
```csharp
var config = new ProvedorIA
{
    Tipo = TipoProvedor.Google,
    Nome = "Gemini Pro",
    ChaveApi = "AIza...",  // Sua chave do Google AI
    Modelo = "gemini-pro",
    MaxTokens = 2048,
    Temperatura = 0.7m,
    UrlBase = "https://generativelanguage.googleapis.com/v1beta/",  // Opcional
    Habilitado = true
};
```

#### Custos (por 1M tokens)
| Modelo | Input | Output |
|--------|-------|--------|
| Gemini 1.5 Pro | $3.50 | $10.50 |
| Gemini Pro | $0.50 | $1.50 |
| Gemini 1.5 Flash | $0.35 | $1.05 |

#### Diferenças Importantes
- API Key vai na URL (query parameter)
- Formato de mensagens diferente (parts array)
- System e user prompts são combinados

## 🏭 ProvedorIAFactory

A factory centraliza a criação de provedores:

```csharp
public class ProvedorIAFactory
{
    public IProvedorIA Criar(ProvedorIA config)
    {
        return config.Tipo switch
        {
            TipoProvedor.OpenAI => new OpenAIProvedor(...),
            TipoProvedor.Anthropic => new ClaudeProvedor(...),
            TipoProvedor.Google => new GeminiProvedor(...),
            _ => throw new NotSupportedException()
        };
    }
}
```

### Uso da Factory

```csharp
// Via configuração do banco
var provedorConfig = await _repo.ObterPorIdAsync(provedorId);
var provedor = _factory.Criar(provedorConfig);

// Ou diretamente
var provedor = _factory.CriarPorTipo(
    TipoProvedor.OpenAI,
    chaveApi: "sk-...",
    modelo: "gpt-4",
    maxTokens: 2000,
    temperatura: 0.7m
);
```

## 📊 ProvedorIAService

Serviço orquestrador que gerencia execução e logging:

```csharp
public class ProvedorIAService
{
    // Executa com provedor padrão do espaço
    public async Task<RespostaIA> ExecutarSolicitacaoAsync(
        Guid espacoId,
        string prompt,
        string? contexto = null,
        Guid? conversaId = null
    )

    // Executa com provedor específico
    public async Task<RespostaIA> ExecutarSolicitacaoComProvedorAsync(
        Guid provedorId,
        string prompt,
        string? contexto = null,
        Guid? conversaId = null
    )

    // Obtém estatísticas
    public async Task<EstatisticasProvedor> ObterEstatisticasAsync(
        Guid provedorId,
        DateTime? dataInicio = null,
        DateTime? dataFim = null
    )
}
```

### Exemplo de Uso

```csharp
// Injetar no construtor
private readonly ProvedorIAService _provedorService;

// Executar solicitação
var resposta = await _provedorService.ExecutarSolicitacaoAsync(
    espacoId: espacoAtual.Id,
    prompt: "Resuma as vendas do dia",
    contexto: "Você é um assistente financeiro",
    conversaId: conversaAtual.Id
);

Console.WriteLine($"Resposta: {resposta.Resposta}");
Console.WriteLine($"Tokens: {resposta.TokensResposta}");
Console.WriteLine($"Custo: {resposta.CustoEstimado:C}");
Console.WriteLine($"Tempo: {resposta.TempoRespostaMs}ms");
```

## 🔐 Segurança

### Armazenamento de Chaves

As chaves de API são armazenadas **cifradas** no banco:

```csharp
// Ao salvar
provedorIA.ChaveApi = _cryptoService.Criptografar("sk-real-key");

// Ao usar
var chaveDecifrada = _cryptoService.Descriptografar(provedorIA.ChaveApi);
```

### Rate Limiting

Cada provedor pode ter limite configurado:

```csharp
provedorIA.LimiteRequisicoesPorMinuto = 60;  // 60 req/min
```

Implementar middleware de rate limiting na API:

```csharp
services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("ia", opt =>
    {
        opt.TokenLimit = provedor.LimiteRequisicoesPorMinuto ?? 100;
        opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
    });
});
```

## 📈 Logging e Observabilidade

Todos os provedores fazem logging estruturado:

```csharp
_logger.LogInformation(
    "Enviando solicitação para {Provedor} - Modelo: {Modelo}, Tokens: {MaxTokens}",
    Nome, _modelo, _maxTokens
);

_logger.LogInformation(
    "Resposta recebida - Tokens Input: {Input}, Output: {Output}, Custo: {Custo:C}",
    tokensPrompt, tokensResposta, custo
);

_logger.LogError(ex, "Erro ao gerar resposta com {Provedor}", Nome);
```

### Métricas Rastreadas

- **SolicitacaoIA**: Prompt, tokens input, timestamp
- **RespostaIA**: Resposta, tokens output, custo, tempo de resposta
- **MetricaUso**: Agregações por dia/mês

## 🧪 Testes

### Teste Unitário

```csharp
[Fact]
public async Task GerarResposta_DeveRetornarTextoValido()
{
    // Arrange
    var mockResponse = new { /* resposta mockada */ };
    var httpClient = CriarHttpClientMock(mockResponse);
    var provedor = new OpenAIProvedor(httpClient, config);

    // Act
    var resposta = await provedor.GerarRespostaAsync("Teste");

    // Assert
    Assert.NotNull(resposta);
}
```

### Teste de Integração

```csharp
[Fact]
public async Task ExecutarSolicitacao_DeveRegistrarNoBanco()
{
    // Arrange
    var service = CreateProvedorIAService();

    // Act
    var resposta = await service.ExecutarSolicitacaoAsync(
        espacoId, "Teste", null, conversaId
    );

    // Assert
    var solicitacao = await _repoSolicitacao.ObterPorIdAsync(resposta.SolicitacaoIAId);
    Assert.NotNull(solicitacao);
    Assert.True(solicitacao.TokensPrompt > 0);
}
```

## 🚀 Boas Práticas

### 1. Escolha do Provedor

- **GPT-4**: Tarefas complexas, raciocínio avançado
- **GPT-3.5**: Tarefas simples, respostas rápidas, custo baixo
- **Claude Opus**: Análise profunda, contextos longos
- **Claude Haiku**: Respostas ultra-rápidas, alta eficiência
- **Gemini Pro**: Alternativa econômica ao GPT-4

### 2. Otimização de Custos

```csharp
// Reutilize contexto quando possível
var contexto = ObterContextoCache(espacoId);

// Limite tokens
config.MaxTokens = 500;  // Ao invés de 2000

// Use modelo mais barato quando adequado
if (tarefaSimples)
    modelo = "gpt-3.5-turbo";
```

### 3. Tratamento de Erros

```csharp
try
{
    var resposta = await provedor.GerarRespostaAsync(prompt, contexto);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
{
    // Rate limit excedido - implementar retry com backoff
    await Task.Delay(TimeSpan.FromSeconds(60));
    // Retry
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
{
    // Chave API inválida - notificar administrador
    _logger.LogCritical("Chave API inválida para {Provedor}", provedor.Nome);
}
```

### 4. Monitoramento

Configure alertas para:
- Custo mensal > limite
- Taxa de erro > 5%
- Tempo de resposta > 10s
- Rate limit atingido

## 📚 Referências

- [OpenAI API Documentation](https://platform.openai.com/docs/api-reference)
- [Anthropic Claude API](https://docs.anthropic.com/claude/reference)
- [Google Gemini API](https://ai.google.dev/docs)

## 🔄 Atualizações

Para adicionar um novo provedor:

1. Criar classe que implementa `IProvedorIA`
2. Adicionar enum em `TipoProvedor`
3. Atualizar `ProvedorIAFactory`
4. Criar testes
5. Documentar custos e particularidades

---

**Última atualização**: 2025-10-17
