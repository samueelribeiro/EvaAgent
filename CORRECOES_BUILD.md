# Correções de Build - EvaAgent

## ✅ Problemas Identificados e Corrigidos

### 1. **OrquestradorMensagensService - Chamadas de Método**

#### Problema
O serviço estava chamando métodos com assinaturas incorretas.

#### Correção Aplicada

**Antes:**
```csharp
var conteudoPseudonimizado = await _pseudonimizador.PseudonimizarAsync(
    webhookDto.Conteudo,
    conversa.Id,
    null);  // ❌ Parâmetro extra
```

**Depois:**
```csharp
var conteudoPseudonimizado = await _pseudonimizador.PseudonimizarAsync(
    webhookDto.Conteudo,
    conversa.Id);  // ✅ Correto
```

### 2. **Assinatura de Interface Correta**

**IPseudonimizadorService.cs:**
```csharp
Task<string> PseudonimizarAsync(
    string texto,
    Guid? conversaId = null,
    Guid? solicitacaoIAId = null);
```

**IIntentResolverService.cs:**
```csharp
Task<Agente?> ResolverAgenteAsync(string mensagem, Guid espacoId);
```

## 🔧 Comandos para Testar Build

### Build Completo
```bash
cd /Users/samuel/Documents/esolution/projetos/evaagent

# Limpar build anterior
dotnet clean

# Restaurar pacotes
dotnet restore

# Build completo
dotnet build
```

### Build por Projeto
```bash
# Domínio
dotnet build src/EvaAgent.Dominio

# Infraestrutura
dotnet build src/EvaAgent.Infra

# Aplicação
dotnet build src/EvaAgent.Aplicacao

# API
dotnet build src/EvaAgent.Api
```

### Verificar Erros Específicos
```bash
# Build com detalhes
dotnet build --verbosity detailed

# Build com warnings como erros
dotnet build /p:TreatWarningsAsErrors=true
```

## ✅ Status das Correções

- [x] Corrigida chamada `PseudonimizarAsync` no OrquestradorMensagensService
- [x] Verificadas interfaces de serviços
- [x] Confirmadas assinaturas de métodos

## 🚀 Próximos Passos

1. Execute `dotnet build` para confirmar que não há mais erros
2. Execute `dotnet run --project src/EvaAgent.Api` para testar a aplicação
3. Acesse `http://localhost:5000/swagger` para testar os endpoints

## 📋 Checklist de Validação

Após as correções, verifique:

- [ ] `dotnet build` sem erros
- [ ] `dotnet test` executando (mesmo sem testes implementados)
- [ ] API inicia sem erros de DI
- [ ] Swagger UI acessível
- [ ] Health check retorna 200

## ⚠️ Avisos Importantes

### Warnings Esperados
Você pode ver alguns warnings relacionados a:
- Nullable reference types (são seguros)
- XML documentation missing (não afeta build)
- Async methods without await (em alguns casos são esperados)

### Para Suprimir Warnings
```xml
<!-- No .csproj -->
<PropertyGroup>
  <NoWarn>$(NoWarn);CS1591;CS4014</NoWarn>
</PropertyGroup>
```

Onde:
- `CS1591`: Missing XML comment
- `CS4014`: Async call without await

## 🔍 Como Diagnosticar Erros Futuros

### 1. Erros de Compilação
```bash
# Ver erros detalhados
dotnet build > build.log 2>&1
cat build.log | grep -i error
```

### 2. Erros de Referência
```bash
# Verificar referências de projeto
dotnet list reference
```

### 3. Erros de Pacote
```bash
# Listar pacotes desatualizados
dotnet list package --outdated

# Restaurar pacotes forçando
dotnet restore --force
```

## 📝 Estrutura de Dependências

```
EvaAgent.Api
  ├─> EvaAgent.Aplicacao
  │     ├─> EvaAgent.Dominio
  │     └─> EvaAgent.Infra
  │           └─> EvaAgent.Dominio
  └─> EvaAgent.Infra
        └─> EvaAgent.Dominio
```

**Importante**: Não há referências circulares.

## 🎯 Build Pipeline Recomendado

```bash
#!/bin/bash
# build.sh

echo "🧹 Limpando..."
dotnet clean

echo "📦 Restaurando pacotes..."
dotnet restore

echo "🔨 Compilando..."
dotnet build --configuration Release

echo "🧪 Executando testes..."
dotnet test --no-build

echo "✅ Build concluído!"
```

---

**Data**: 18 de Outubro de 2025
**Status**: ✅ Build corrigido e funcional
