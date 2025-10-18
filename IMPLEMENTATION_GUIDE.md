# Guia de Implementação - Próximos Passos

Este documento complementa o README.md e fornece instruções detalhadas para completar a implementação do projeto.

## 📋 Status da Implementação

### ✅ Já Implementado

1. **Estrutura do Projeto** - Solução .NET com camadas (Api, Aplicacao, Dominio, Infra, Shared, Tests)
2. **Domínio Completo** - Todas as entidades, enums, interfaces e value objects
3. **Infraestrutura Base**:
   - DbContext (EF Core + PostgreSQL)
   - RepositorioBase genérico
   - CryptoService (AES-256 + SHA-256)
   - PseudonimizadorService (LGPD)
   - Configuração de exemplo (UsuarioConfiguration)
4. **Docker Compose** - PostgreSQL, pgAdmin, Redis, Seq, Jaeger
5. **README Completo** - Documentação detalhada da arquitetura

### 🔨 A Implementar

## 1. Configurações EF Core (Fluent API)

Crie configurações para todas as entidades no padrão `src/EvaAgent.Infra/Data/Configurations/`:

```csharp
// Exemplo: EspacoConfiguration.cs
public class EspacoConfiguration : IEntityTypeConfiguration<Espaco>
{
    public void Configure(EntityTypeBuilder<Espaco> builder)
    {
        builder.ToTable("espaco");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Slug)
            .HasMaxLength(200);

        builder.HasIndex(e => e.Slug)
            .IsUnique()
            .HasDatabaseName("idx_espaco_slug");

        // Relacionamento hierárquico
        builder.HasOne(e => e.EspacoPai)
            .WithMany(e => e.SubEspacos)
            .HasForeignKey(e => e.EspacoPaiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.UsuarioEspacos)
            .WithOne(ue => ue.Espaco)
            .HasForeignKey(ue => ue.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**Configurações Necessárias** (crie uma por entidade):
- PapelConfiguration
- EspacoConfiguration
- UsuarioEspacoConfiguration
- CanalConfiguration
- ReceptorConfiguration
- ProvedorIAConfiguration
- SolicitacaoIAConfiguration
- RespostaIAConfiguration
- AgenteConfiguration
- IntencaoAgenteConfiguration
- SistemaConfiguration
- ConectorConfiguration
- ConsultaNegocioConfiguration
- AcaoNegocioConfiguration
- ConversaConfiguration
- MensagemConfiguration
- RegistroPseudonimizacaoConfiguration
- ConsentimentoLGPDConfiguration
- MemoriaCurtoPrazoConfiguration
- MemoriaLongoPrazoConfiguration
- GrupoTreinamentoConfiguration
- DocumentoTreinamentoConfiguration
- TarefaAgendadaConfiguration
- FilaMensagemConfiguration
- FilaDeadLetterConfiguration
- RegistroAuditoriaConfiguration
- RegistroErroConfiguration
- MetricaUsoConfiguration
- RegistroExecucaoConfiguration

## 2. Migrations e Seeds

### Criar Migration Inicial

```bash
cd src/EvaAgent.Api
dotnet ef migrations add InitialCreate --project ../EvaAgent.Infra --output Data/Migrations
```

### Criar DbInitializer

```csharp
// src/EvaAgent.Infra/Data/DbInitializer.cs
public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context, ICryptoService crypto)
    {
        // 1. Papéis
        if (!await context.Papeis.AnyAsync())
        {
            await context.Papeis.AddRangeAsync(
                new Papel
                {
                    Nome = "Administrador",
                    Tipo = TipoPapel.Administrador,
                    Permissoes = JsonSerializer.Serialize(new[] { "*" })
                },
                new Papel
                {
                    Nome = "Membro",
                    Tipo = TipoPapel.Membro,
                    Permissoes = JsonSerializer.Serialize(new[] { "read", "write" })
                }
            );
            await context.SaveChangesAsync();
        }

        // 2. Usuário Admin
        if (!await context.Usuarios.AnyAsync())
        {
            var admin = new Usuario
            {
                Nome = "Administrador",
                Email = "admin@evaagent.local",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                EmailVerificado = true
            };
            await context.Usuarios.AddAsync(admin);
            await context.SaveChangesAsync();
        }

        // 3. Espaço Padrão
        if (!await context.Espacos.AnyAsync())
        {
            var espaco = new Espaco
            {
                Nome = "Espaço Demo",
                Slug = "espaco-demo",
                Descricao = "Espaço de demonstração"
            };
            await context.Espacos.AddAsync(espaco);
            await context.SaveChangesAsync();

            // Vincular admin ao espaço
            var admin = await context.Usuarios.FirstAsync(u => u.Email == "admin@evaagent.local");
            var papelAdmin = await context.Papeis.FirstAsync(p => p.Tipo == TipoPapel.Administrador);

            await context.UsuarioEspacos.AddAsync(new UsuarioEspaco
            {
                UsuarioId = admin.Id,
                EspacoId = espaco.Id,
                PapelId = papelAdmin.Id,
                AceitoEm = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        // 4. Provedores IA (desabilitados por padrão)
        if (!await context.ProvedoresIA.AnyAsync())
        {
            var espaco = await context.Espacos.FirstAsync();

            await context.ProvedoresIA.AddRangeAsync(
                new ProvedorIA
                {
                    EspacoId = espaco.Id,
                    Nome = "OpenAI GPT-4",
                    Tipo = TipoProvedor.OpenAI,
                    Modelo = "gpt-4",
                    MaxTokens = 2000,
                    Temperatura = 0.7m,
                    Habilitado = false // Configure manualmente
                },
                new ProvedorIA
                {
                    EspacoId = espaco.Id,
                    Nome = "Claude 3 Opus",
                    Tipo = TipoProvedor.Anthropic,
                    Modelo = "claude-3-opus-20240229",
                    MaxTokens = 2000,
                    Temperatura = 0.7m,
                    Habilitado = false
                },
                new ProvedorIA
                {
                    EspacoId = espaco.Id,
                    Nome = "Gemini Pro",
                    Tipo = TipoProvedor.Google,
                    Modelo = "gemini-pro",
                    MaxTokens = 2000,
                    Temperatura = 0.7m,
                    Habilitado = false
                }
            );
            await context.SaveChangesAsync();
        }

        // 5. Agentes Padrão
        if (!await context.Agentes.AnyAsync())
        {
            var espaco = await context.Espacos.FirstAsync();

            await context.Agentes.AddRangeAsync(
                new Agente
                {
                    EspacoId = espaco.Id,
                    Nome = "Agente Financeiro",
                    Tipo = TipoAgente.Financeiro,
                    PalavrasChaveJson = JsonSerializer.Serialize(new[] {
                        "venda", "vendas", "saldo", "financeiro", "boleto", "pagamento",
                        "cobrança", "fatura", "receber", "pagar"
                    }),
                    PromptSistema = "Você é um assistente especializado em questões financeiras.",
                    Prioridade = 10
                },
                new Agente
                {
                    EspacoId = espaco.Id,
                    Nome = "Agente Multipropriedade",
                    Tipo = TipoAgente.Multipropriedade,
                    PalavrasChaveJson = JsonSerializer.Serialize(new[] {
                        "multipropriedade", "reserva", "semana", "período", "disponibilidade"
                    }),
                    PromptSistema = "Você é um assistente especializado em multipropriedade.",
                    Prioridade = 9
                },
                new Agente
                {
                    EspacoId = espaco.Id,
                    Nome = "Agente Hotelaria",
                    Tipo = TipoAgente.Hotelaria,
                    PalavrasChaveJson = JsonSerializer.Serialize(new[] {
                        "hotel", "quarto", "hospedagem", "check-in", "check-out", "diária"
                    }),
                    PromptSistema = "Você é um assistente especializado em hotelaria.",
                    Prioridade = 8
                },
                new Agente
                {
                    EspacoId = espaco.Id,
                    Nome = "Agente Geral",
                    Tipo = TipoAgente.Geral,
                    PalavrasChaveJson = JsonSerializer.Serialize(new[] { "*" }),
                    PromptSistema = "Você é um assistente geral. Responda de forma amigável e útil.",
                    Prioridade = 1 // Menor prioridade (fallback)
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
```

## 3. Provedores de IA

### OpenAIProvedor

```csharp
// src/EvaAgent.Infra/Servicos/IA/OpenAIProvedor.cs
public class OpenAIProvedor : IProvedorIA
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelo;
    private readonly int? _maxTokens;
    private readonly decimal? _temperatura;

    public string Nome => "OpenAI";

    public OpenAIProvedor(HttpClient httpClient, ProvedorIA config)
    {
        _httpClient = httpClient;
        _apiKey = config.ChaveApi!;
        _modelo = config.Modelo ?? "gpt-4";
        _maxTokens = config.MaxTokens;
        _temperatura = config.Temperatura;

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
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

        var response = await _httpClient.PostAsJsonAsync(
            "https://api.openai.com/v1/chat/completions",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>(cancellationToken);

        var resposta = result!.Choices[0].Message.Content;
        var tokensPrompt = result.Usage.PromptTokens;
        var tokensResposta = result.Usage.CompletionTokens;

        // Custo estimado (GPT-4: $0.03/1K input, $0.06/1K output)
        var custo = (tokensPrompt * 0.03m / 1000) + (tokensResposta * 0.06m / 1000);

        return (resposta, tokensPrompt, tokensResposta, custo);
    }

    private class OpenAIResponse
    {
        public List<Choice> Choices { get; set; } = new();
        public Usage Usage { get; set; } = new();
    }

    private class Choice
    {
        public Message Message { get; set; } = new();
    }

    private class Message
    {
        public string Content { get; set; } = string.Empty;
    }

    private class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }
    }
}
```

### ClaudeProvedor e GeminiProvedor

Implemente de forma similar ao OpenAIProvedor, seguindo a documentação oficial:
- Claude: https://docs.anthropic.com/claude/reference/messages_post
- Gemini: https://cloud.google.com/vertex-ai/docs/generative-ai/model-reference/gemini

## 4. Conectores Dapper

```csharp
// src/EvaAgent.Infra/Servicos/Conectores/DbConnectionFactory.cs
public class DbConnectionFactory
{
    public IDbConnection CriarConexao(TipoBancoDados tipo, string connectionString)
    {
        return tipo switch
        {
            TipoBancoDados.SqlServer => new SqlConnection(connectionString),
            TipoBancoDados.PostgreSQL => new NpgsqlConnection(connectionString),
            TipoBancoDados.Oracle => new OracleConnection(connectionString),
            TipoBancoDados.MySQL => new MySqlConnection(connectionString),
            _ => throw new NotSupportedException($"Tipo de banco '{tipo}' não suportado")
        };
    }
}

// src/EvaAgent.Infra/Servicos/Conectores/ConsultaExecutorDapper.cs
public class ConsultaExecutorDapper
{
    private readonly IRepositorioBase<ConsultaNegocio> _repoConsulta;
    private readonly IRepositorioBase<Conector> _repoConector;
    private readonly DbConnectionFactory _factory;
    private readonly ICryptoService _crypto;

    public async Task<IEnumerable<dynamic>> ExecutarAsync(
        Guid consultaId,
        Dictionary<string, object> parametros)
    {
        var consulta = await _repoConsulta.ObterPorIdAsync(consultaId);
        if (consulta == null)
            throw new ArgumentException("Consulta não encontrada", nameof(consultaId));

        var sistema = consulta.Sistema;
        var conector = sistema.Conectores.FirstOrDefault(c => c.Tipo == TipoConector.BancoDados && c.Habilitado);
        if (conector == null)
            throw new InvalidOperationException("Nenhum conector de banco de dados habilitado para este sistema");

        var connectionString = _crypto.Descriptografar(conector.StringConexao!);

        using var conn = _factory.CriarConexao(conector.TipoBancoDados!.Value, connectionString);
        conn.Open();

        var resultado = await conn.QueryAsync(consulta.QuerySql, parametros, commandTimeout: conector.TimeoutSegundos);

        return resultado;
    }
}
```

## 5. Sistema de Agentes

```csharp
// src/EvaAgent.Infra/Servicos/Agentes/IntentResolverService.cs
public class IntentResolverService : IIntentResolverService
{
    private readonly IRepositorioBase<Agente> _repositorio;

    public async Task<(Agente? Agente, decimal Confianca)> ResolverIntencaoAsync(
        string mensagem,
        Guid espacoId,
        CancellationToken cancellationToken = default)
    {
        var agentes = await _repositorio.BuscarAsync(
            a => a.EspacoId == espacoId && a.Habilitado,
            cancellationToken);

        var tokens = Tokenizar(mensagem);
        var scores = new List<(Agente Agente, decimal Score)>();

        foreach (var agente in agentes)
        {
            var palavrasChave = JsonSerializer.Deserialize<string[]>(agente.PalavrasChaveJson ?? "[]") ?? Array.Empty<string>();

            var score = CalcularScore(tokens, palavrasChave);
            scores.Add((agente, score));
        }

        var melhor = scores.OrderByDescending(s => s.Score).FirstOrDefault();

        // Threshold mínimo de 0.6
        return melhor.Score >= 0.6m
            ? (melhor.Agente, melhor.Score)
            : (null, 0);
    }

    private static List<string> Tokenizar(string mensagem)
    {
        return mensagem
            .ToLowerInvariant()
            .Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    private static decimal CalcularScore(List<string> tokens, string[] palavrasChave)
    {
        if (palavrasChave.Contains("*"))
            return 0.5m; // Agente geral tem score baixo (fallback)

        var matches = tokens.Count(t => palavrasChave.Contains(t, StringComparer.OrdinalIgnoreCase));
        return matches > 0 ? (decimal)matches / tokens.Count : 0m;
    }
}
```

## 6. appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=evaagent;Username=postgres;Password=postgres123"
  },
  "Jwt": {
    "SecretKey": "SuaChaveSecretaSuperSeguraMinimo256Bits!",
    "Issuer": "https://evaagent.local",
    "Audience": "https://evaagent.local",
    "ExpirationMinutes": 60
  },
  "Crypto": {
    "Key": "SuaChaveAES256Base64Aqui==",
    "IV": "SeuIVAES128Base64=="
  },
  "OpenAI": {
    "ApiKey": "",
    "Organization": ""
  },
  "Anthropic": {
    "ApiKey": ""
  },
  "Google": {
    "ApplicationCredentials": "/path/to/credentials.json",
    "ProjectId": ""
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Seq" ],
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "Seq",
        "Args": { "serverUrl": "http://localhost:5341" }
      }
    ]
  }
}
```

## 7. Program.cs (API)

```csharp
using Microsoft.EntityFrameworkCore;
using EvaAgent.Infra.Data.Contexts;
using EvaAgent.Infra.Data.Repositories;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;
using EvaAgent.Infra.Servicos.LGPD;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped(typeof(IRepositorioBase<>), typeof(RepositorioBase<>));

// Services
builder.Services.AddSingleton<ICryptoService>(sp =>
{
    var key = builder.Configuration["Crypto:Key"]!;
    var iv = builder.Configuration["Crypto:IV"]!;
    return new CryptoService(key, iv);
});

builder.Services.AddScoped<IPseudonimizadorService, PseudonimizadorService>();

// TODO: Adicionar outros serviços (IA, Agentes, Conectores, etc.)

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var crypto = scope.ServiceProvider.GetRequiredService<ICryptoService>();

    await context.Database.MigrateAsync();
    await DbInitializer.SeedAsync(context, crypto);
}

app.Run();

public partial class Program { } // For testing
```

## 8. Testes

### Exemplo: Teste Unitário (Domínio)

```csharp
// tests/EvaAgent.Dominio.Tests/Servicos/CryptoServiceTests.cs
public class CryptoServiceTests
{
    private readonly ICryptoService _cryptoService;

    public CryptoServiceTests()
    {
        // Chaves de teste (não usar em produção!)
        var key = Convert.ToBase64String(new byte[32]); // 256 bits
        var iv = Convert.ToBase64String(new byte[16]);  // 128 bits
        _cryptoService = new CryptoService(key, iv);
    }

    [Fact]
    public void DeveCriptografarEDescriptografarTexto()
    {
        // Arrange
        var textoOriginal = "José Silva";

        // Act
        var textoCifrado = _cryptoService.Criptografar(textoOriginal);
        var textoDecifrado = _cryptoService.Descriptografar(textoCifrado);

        // Assert
        Assert.NotEqual(textoOriginal, textoCifrado);
        Assert.Equal(textoOriginal, textoDecifrado);
    }

    [Fact]
    public void DeveGerarHashConsistente()
    {
        // Arrange
        var texto = "123.456.789-00";

        // Act
        var hash1 = _cryptoService.GerarHash(texto);
        var hash2 = _cryptoService.GerarHash(texto);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void DeveVerificarHashCorretamente()
    {
        // Arrange
        var texto = "teste@exemplo.com";
        var hash = _cryptoService.GerarHash(texto);

        // Act & Assert
        Assert.True(_cryptoService.VerificarHash(texto, hash));
        Assert.False(_cryptoService.VerificarHash("outro@exemplo.com", hash));
    }
}
```

### Exemplo: Teste de Integração (Repositório)

```csharp
// tests/EvaAgent.Infra.Tests/Repositories/RepositorioBaseTests.cs
public class RepositorioBaseTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public RepositorioBaseTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeveAdicionarERecuperarUsuario()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepositorioBase<Usuario>>();

        var usuario = new Usuario
        {
            Nome = "Teste",
            Email = $"teste{Guid.NewGuid()}@exemplo.com",
            SenhaHash = "hash"
        };

        // Act
        await repo.AdicionarAsync(usuario);
        var recuperado = await repo.ObterPorIdAsync(usuario.Id);

        // Assert
        Assert.NotNull(recuperado);
        Assert.Equal("Teste", recuperado.Nome);
    }
}

// Fixture
public class DatabaseFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }

    public DatabaseFixture()
    {
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql("Host=localhost;Database=evaagent_test;Username=postgres;Password=postgres123"));

        services.AddScoped(typeof(IRepositorioBase<>), typeof(RepositorioBase<>));

        ServiceProvider = services.BuildServiceProvider();

        // Criar banco e aplicar migrations
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }

    public void Dispose()
    {
        // Cleanup
    }
}
```

## 9. Próximos Passos Prioritários

1. **Implementar todas as Configurations do EF** (28 arquivos)
2. **Criar e aplicar Migrations**
3. **Implementar ClaudeProvedor e GeminiProvedor**
4. **Implementar Agentes Especialistas** (5 classes)
5. **Implementar ConsultaExecutor e AcaoExecutor**
6. **Implementar Controllers da API** (Webhook, Chat, Consultas, Ações, Admin)
7. **Implementar Middlewares** (Auth, RateLimit, Errors, CORS)
8. **Implementar Sistema de Memória e RAG**
9. **Implementar Fila e Jobs (HostedService)**
10. **Escrever Testes** (cobertura ≥ 80%)

## 10. Comandos Úteis

```bash
# Build
dotnet build

# Restaurar pacotes
dotnet restore

# Criar migration
dotnet ef migrations add NomeMigration --project src/EvaAgent.Infra --startup-project src/EvaAgent.Api

# Aplicar migrations
dotnet ef database update --project src/EvaAgent.Infra --startup-project src/EvaAgent.Api

# Remover última migration (se não aplicada)
dotnet ef migrations remove --project src/EvaAgent.Infra --startup-project src/EvaAgent.Api

# Executar testes
dotnet test

# Executar com coverage
dotnet test --collect:"XPlat Code Coverage"

# Executar API
dotnet run --project src/EvaAgent.Api

# Watch mode (desenvolvimento)
dotnet watch run --project src/EvaAgent.Api
```

## 11. Recursos Adicionais

- [Entity Framework Core Docs](https://learn.microsoft.com/ef/core/)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [xUnit Documentation](https://xunit.net/)
- [Moq Framework](https://github.com/moq/moq4)
- [FluentAssertions](https://fluentassertions.com/)

---

**Boa implementação!** 🚀
