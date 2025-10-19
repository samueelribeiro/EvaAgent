# Testes - EvaAgent

Este diretório contém todos os testes automatizados do projeto EvaAgent.

## Estrutura de Testes

```
tests/
├── EvaAgent.Dominio.Tests/      # Testes unitários da camada de domínio
│   ├── Entidades/                # Testes de entidades
│   └── Servicos/                 # Testes de serviços de domínio
├── EvaAgent.Aplicacao.Tests/    # Testes unitários da camada de aplicação
│   └── Services/                 # Testes de serviços de aplicação
├── EvaAgent.Infra.Tests/        # Testes de integração da infraestrutura
│   ├── Repositories/             # Testes de repositórios
│   └── Servicos/                 # Testes de serviços de infraestrutura
└── EvaAgent.Api.Tests/          # Testes E2E e de controllers
    └── Controllers/              # Testes de controllers
```

## Tecnologias Utilizadas

- **xUnit**: Framework de testes
- **Moq**: Biblioteca de mocking
- **FluentAssertions**: Assertions mais legíveis
- **Microsoft.AspNetCore.Mvc.Testing**: Testes de integração para API
- **Testcontainers**: Containers Docker para testes de integração
- **coverlet**: Cobertura de código

## Executar Testes

### Todos os testes
```bash
dotnet test
```

### Testes de um projeto específico
```bash
dotnet test tests/EvaAgent.Dominio.Tests
dotnet test tests/EvaAgent.Aplicacao.Tests
dotnet test tests/EvaAgent.Infra.Tests
dotnet test tests/EvaAgent.Api.Tests
```

### Com cobertura de código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Gerar relatório de cobertura
```bash
# Instalar ferramenta (apenas uma vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório HTML
reportgenerator \
  "-reports:**/coverage.cobertura.xml" \
  "-targetdir:coverage-report" \
  "-reporttypes:Html"

# Abrir relatório
open coverage-report/index.html  # macOS
```

### Modo watch (desenvolvimento)
```bash
dotnet watch test --project tests/EvaAgent.Dominio.Tests
```

## Tipos de Testes

### 1. Testes Unitários (Domínio e Aplicação)

**Objetivo**: Testar lógica de negócio isoladamente

**Características**:
- Sem dependências externas (banco, API, etc)
- Uso de mocks para dependências
- Execução rápida
- Alta cobertura esperada (≥ 90%)

**Exemplo**:
```csharp
public class CryptoServiceTests
{
    [Fact]
    public void DeveCriptografarEDescriptografarTexto()
    {
        // Arrange
        var service = new CryptoService(key, iv);
        var texto = "José Silva";

        // Act
        var cifrado = service.Criptografar(texto);
        var decifrado = service.Descriptografar(cifrado);

        // Assert
        decifrado.Should().Be(texto);
    }
}
```

### 2. Testes de Integração (Infraestrutura)

**Objetivo**: Testar integração com recursos externos

**Características**:
- Usa banco de dados real (Testcontainers)
- Testa repositórios, conectores, etc
- Execução mais lenta
- Cobertura esperada (≥ 70%)

**Exemplo**:
```csharp
public class RepositorioBaseTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    [Fact]
    public async Task DeveAdicionarERecuperarEntidade()
    {
        // Arrange
        var repo = _fixture.GetRepository<Usuario>();
        var usuario = new Usuario { Nome = "Teste" };

        // Act
        await repo.AdicionarAsync(usuario);
        var recuperado = await repo.ObterPorIdAsync(usuario.Id);

        // Assert
        recuperado.Should().NotBeNull();
    }
}
```

### 3. Testes E2E (API)

**Objetivo**: Testar fluxo completo da aplicação

**Características**:
- Usa `WebApplicationFactory`
- Testa endpoints end-to-end
- Simula requisições HTTP reais
- Cobertura esperada (≥ 75%)

**Exemplo**:
```csharp
public class HealthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    [Fact]
    public async Task Health_DeveRetornar200()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
```

## Metas de Cobertura

| Projeto | Meta | Status |
|---------|------|--------|
| EvaAgent.Dominio | ≥ 90% | 🟡 Em desenvolvimento |
| EvaAgent.Aplicacao | ≥ 85% | 🟡 Em desenvolvimento |
| EvaAgent.Infra | ≥ 70% | 🟡 Em desenvolvimento |
| EvaAgent.Api | ≥ 75% | 🟡 Em desenvolvimento |
| **Geral** | **≥ 80%** | 🟡 Em desenvolvimento |

## Boas Práticas

### Nomenclatura de Testes
```csharp
// Padrão: Deve<Ação><Condição>
[Fact]
public void DeveCriptografarTextoCorretamente() { }

[Fact]
public void DeveRetornarErro_QuandoTextoVazio() { }

[Theory]
[InlineData("")]
[InlineData(null)]
public void NaoDeveAceitarTextoInvalido(string texto) { }
```

### Organização AAA (Arrange-Act-Assert)
```csharp
[Fact]
public void ExemploAAA()
{
    // Arrange - Preparar dados de teste
    var service = new MinhaService();
    var input = "teste";

    // Act - Executar ação
    var resultado = service.Processar(input);

    // Assert - Verificar resultado
    resultado.Should().Be("esperado");
}
```

### Uso de FluentAssertions
```csharp
// Ao invés de:
Assert.Equal(expected, actual);
Assert.True(condition);

// Prefira:
actual.Should().Be(expected);
condition.Should().BeTrue();
list.Should().HaveCount(3);
string.Should().NotBeNullOrEmpty();
```

### Uso de Mocks
```csharp
// Criar mock
var mock = new Mock<IRepositorio>();

// Setup de retorno
mock.Setup(x => x.ObterPorId(It.IsAny<Guid>()))
    .ReturnsAsync(new Usuario());

// Verificar chamadas
mock.Verify(x => x.Salvar(It.IsAny<Usuario>()), Times.Once);
```

## Test Fixtures e Helpers

### DatabaseFixture (para testes de integração)
```csharp
public class DatabaseFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }

    public DatabaseFixture()
    {
        // Configurar Testcontainers, DbContext, etc
    }

    public void Dispose()
    {
        // Cleanup
    }
}
```

### Builders (para criar entidades de teste)
```csharp
public class UsuarioBuilder
{
    private string _nome = "Teste";
    private string _email = "teste@exemplo.com";

    public UsuarioBuilder ComNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public Usuario Build()
    {
        return new Usuario
        {
            Nome = _nome,
            Email = _email,
            SenhaHash = "hash"
        };
    }
}

// Uso:
var usuario = new UsuarioBuilder()
    .ComNome("João")
    .Build();
```

## Debugging de Testes

### Visual Studio Code
1. Abrir painel de testes (Testing)
2. Clicar em "Debug Test" ao lado do teste

### Linha de comando
```bash
# Executar teste específico
dotnet test --filter "FullyQualifiedName~CryptoServiceTests"

# Verbose para mais detalhes
dotnet test --logger "console;verbosity=detailed"
```

## CI/CD

Os testes são executados automaticamente em:
- Pull Requests
- Push para branch main
- Releases

Pipeline mínimo esperado:
```yaml
- dotnet restore
- dotnet build
- dotnet test --collect:"XPlat Code Coverage"
- reportgenerator (gerar relatório)
```

## Recursos Adicionais

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Testcontainers Documentation](https://dotnet.testcontainers.org/)

## Contribuindo

Ao adicionar novos testes:
1. Seguir padrões de nomenclatura
2. Usar AAA pattern
3. Preferir FluentAssertions
4. Manter metas de cobertura
5. Adicionar comentários quando lógica for complexa
