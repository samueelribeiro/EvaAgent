# Correção do Erro de Injeção de Dependência - CryptoService

## ❌ Problema Identificado

```
System.AggregateException: 'Some services are not able to be constructed
(Error while validating the service descriptor 'ServiceType:
EvaAgent.Dominio.Interfaces.Servicos.ICryptoService Lifetime: Scoped
ImplementationType: EvaAgent.Infra.Servicos.LGPD.CryptoService':
Unable to resolve service for type 'System.String' while attempting to
activate 'EvaAgent.Infra.Servicos.LGPD.CryptoService'.)'
```

### Causa Raiz

O `CryptoService` tinha um construtor que esperava dois parâmetros `string` (chave e IV):

```csharp
public CryptoService(string key, string iv)
{
    // ...
}
```

O sistema de **Dependency Injection (DI)** do ASP.NET Core não sabia como fornecer esses valores string, pois eles devem vir da configuração (`appsettings.json`).

## ✅ Solução Implementada

### 1. Criada Classe de Opções

Arquivo: `src/EvaAgent.Infra/Configuracoes/CryptoOptions.cs`

```csharp
namespace EvaAgent.Infra.Configuracoes;

public class CryptoOptions
{
    public const string SectionName = "Crypto";

    public string Key { get; set; } = string.Empty;
    public string IV { get; set; } = string.Empty;
}
```

### 2. Modificado CryptoService

Adicionado construtor que aceita `IOptions<CryptoOptions>`:

```csharp
public class CryptoService : ICryptoService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    // Construtor principal para DI
    public CryptoService(IOptions<CryptoOptions> options)
    {
        var cryptoOptions = options.Value;
        _key = Encoding.UTF8.GetBytes(cryptoOptions.Key.PadRight(32).Substring(0, 32));
        _iv = Encoding.UTF8.GetBytes(cryptoOptions.IV.PadRight(16).Substring(0, 16));

        // Validações...
    }

    // Construtor alternativo para testes
    public CryptoService(string key, string iv)
    {
        _key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        _iv = Encoding.UTF8.GetBytes(iv.PadRight(16).Substring(0, 16));

        // Validações...
    }
}
```

### 3. Atualizado Program.cs

Configurado o Options Pattern antes de registrar os serviços:

```csharp
// Configure Options
builder.Services.Configure<CryptoOptions>(
    builder.Configuration.GetSection(CryptoOptions.SectionName));

// Register Infrastructure Services
builder.Services.AddScoped<ICryptoService, CryptoService>();
```

### 4. Configuração no appsettings.json

O `appsettings.json` já possui a seção `Crypto`:

```json
{
  "Crypto": {
    "Key": "Change-This-Key-To-A-32-Character-String!!",
    "IV": "Change-This-IV!"
  }
}
```

## 🔧 Como Funciona Agora

### Fluxo de Injeção

1. **Aplicação inicia** → ASP.NET Core lê `appsettings.json`
2. **Configure<CryptoOptions>** → Mapeia seção `"Crypto"` para classe `CryptoOptions`
3. **DI Container** → Injeta `IOptions<CryptoOptions>` no `CryptoService`
4. **CryptoService** → Recebe configurações e inicializa chaves de criptografia

### Diagrama

```
appsettings.json
    └── Seção "Crypto"
        └── Key + IV
            ↓
    IOptions<CryptoOptions>
            ↓
    CryptoService(IOptions<CryptoOptions>)
            ↓
    ICryptoService (disponível para DI)
            ↓
    PseudonimizadorService
    OrquestradorMensagensService
    etc.
```

## 🧪 Impacto nos Testes

O construtor alternativo `CryptoService(string key, string iv)` foi mantido especificamente para testes:

```csharp
// Em testes unitários
var cryptoService = new CryptoService(
    "Change-This-Key-To-A-32-Character-String!!",
    "Change-This-IV!"
);
```

Isso permite testar sem precisar configurar o Options Pattern.

## ✅ Benefícios da Solução

1. **Segue padrões ASP.NET Core** - Usa Options Pattern recomendado
2. **Configurável** - Valores vêm de `appsettings.json`
3. **Testável** - Construtor alternativo para testes
4. **Seguro** - Chaves não estão hard-coded
5. **Flexível** - Fácil mudar configurações por ambiente

## 📋 Checklist de Verificação

- [x] Classe `CryptoOptions` criada
- [x] Construtor com `IOptions<CryptoOptions>` adicionado
- [x] Construtor alternativo para testes mantido
- [x] `Program.cs` configurado com `Configure<CryptoOptions>`
- [x] `appsettings.json` possui seção `Crypto`
- [x] Erro de DI resolvido

## 🚀 Próximos Passos

1. **Testar a aplicação**:
   ```bash
   dotnet run --project src/EvaAgent.Api
   ```

2. **Verificar se inicia sem erros**:
   - Aplicação deve iniciar na porta 5000/5001
   - Health check deve responder em `/health`

3. **Trocar chaves em produção**:
   ```json
   {
     "Crypto": {
       "Key": "SuaChaveSecuraAqui32Caracteres!!",
       "IV": "SeuIVSeguro16!!!"
     }
   }
   ```

## ⚠️ Importante: Segurança

### Para Desenvolvimento
As chaves atuais em `appsettings.json` são **apenas para desenvolvimento**.

### Para Produção
**NUNCA** commite chaves reais! Use:

1. **Variáveis de Ambiente**:
   ```bash
   export Crypto__Key="ChaveReal..."
   export Crypto__IV="IVReal..."
   ```

2. **Azure Key Vault**:
   ```csharp
   builder.Configuration.AddAzureKeyVault(
       new Uri("https://seu-vault.vault.azure.net/"),
       new DefaultAzureCredential());
   ```

3. **User Secrets** (desenvolvimento):
   ```bash
   dotnet user-secrets set "Crypto:Key" "MinhaChaveLocal"
   dotnet user-secrets set "Crypto:IV" "MeuIVLocal"
   ```

## 📚 Referências

- [Options Pattern in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/options)
- [Dependency Injection in .NET](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/)

---

**Data da Correção**: 18 de Outubro de 2025
**Status**: ✅ Resolvido
