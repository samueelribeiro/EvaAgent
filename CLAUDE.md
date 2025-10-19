# CLAUDE.md

Este arquivo fornece orientações ao Claude Code (claude.ai/code) ao trabalhar com código neste repositório.

## Visão Geral do Projeto

**EvaAgent** é uma plataforma orquestradora de IA inteligente que recebe mensagens de múltiplos canais (WhatsApp, Telegram, WebChat, Email, SMS), identifica a intenção usando NLP, delega para agentes especializados por domínio de negócio, executa consultas e ações em sistemas legados, e implementa pseudonimização de dados em conformidade com a LGPD antes de enviar para LLMs.

**Stack Tecnológica**: .NET 10, ASP.NET Core, Entity Framework Core, PostgreSQL, Dapper (multi-database), Redis, Docker

**Arquitetura**: Clean Architecture + Domain-Driven Design (DDD)

## Estrutura do Projeto

```
src/
├── EvaAgent.Api/           # Controllers da API, Program.cs, middleware
├── EvaAgent.Aplicacao/     # Serviços de aplicação, DTOs, casos de uso
├── EvaAgent.Dominio/       # Entidades de domínio, interfaces, enums (30 entidades)
├── EvaAgent.Infra/         # EF Core, repositórios, serviços externos
└── EvaAgent.Shared/        # Utilitários e constantes compartilhadas

tests/
├── EvaAgent.Dominio.Tests/
├── EvaAgent.Aplicacao.Tests/
├── EvaAgent.Infra.Tests/
└── EvaAgent.Api.Tests/

deploy/                     # docker-compose.yml para PostgreSQL, Redis, Seq, Jaeger
```

## Conceitos Principais da Arquitetura

### 1. Sistema Multi-Agente
- **IntentResolverService** usa correspondência de palavras-chave para rotear mensagens para agentes especializados
- Agentes são específicos por domínio (Financeiro, Hotelaria, Multipropriedade, Geral)
- Cada agente possui palavras-chave configuráveis e níveis de prioridade
- Fallback para agente geral se score de confiança < 0.6

### 2. Conformidade com LGPD (Privacidade de Dados)
- **PseudonimizadorService** detecta e mascara automaticamente dados sensíveis (CPF, CNPJ, emails, nomes)
- Dados são pseudonimizados ANTES de enviar para LLMs externos
- GUIDs substituem dados sensíveis, armazenados criptografados na tabela `registro_pseudonimizacao`
- Após resposta da IA, dados são des-pseudonimizados para apresentação ao usuário
- Todas as operações são registradas para trilhas de auditoria

### 3. Integração Multi-Provedor de IA
- Padrão Factory para provedores de IA: OpenAI, Anthropic Claude, Google Gemini
- Cada provedor implementa a interface `IProvedorIA`
- Suporta modelos customizados, temperatura, max tokens por espaço (tenant)
- Rate limiting e rastreamento de custos integrados

### 4. Conectividade Multi-Database
- **Dapper** para consultas dinâmicas em sistemas legados (SQL Server, PostgreSQL, Oracle)
- **DbConnectionFactory** cria conexões baseado no enum `TipoBancoDados`
- Connection strings são criptografadas com AES-256 no banco de dados
- **ConsultaExecutorDapper** executa consultas de negócio com parametrização

### 5. Multi-Tenancy (Espaços)
- Espaços hierárquicos (tabela `espaco` com `espaco_pai_id`)
- RBAC (Controle de Acesso Baseado em Papéis) via tabelas `papel` e `usuario_espaco`
- Todas as consultas são filtradas por `espaco_id`

## Comandos de Desenvolvimento

### Build e Execução
```bash
# Restaurar pacotes
dotnet restore

# Build da solução completa
dotnet build

# Executar API (modo desenvolvimento com hot reload)
dotnet watch run --project src/EvaAgent.Api

# Executar API (modo produção)
dotnet run --project src/EvaAgent.Api
```

### Acessar Documentação Swagger
Após iniciar a API, acesse:
- **Swagger UI**: `http://localhost:5000/swagger`
- **OpenAPI JSON**: `http://localhost:5000/api-docs/v1/swagger.json`

A documentação Swagger fornece:
- Interface interativa para testar endpoints
- Documentação completa de todos os endpoints
- Exemplos de request/response
- Esquemas de dados (DTOs)
- Botão "Try it out" para testar diretamente na interface

### Migrations de Banco de Dados
```bash
# Criar uma nova migration
dotnet ef migrations add <NomeDaMigration> \
  --project src/EvaAgent.Infra \
  --startup-project src/EvaAgent.Api \
  --context AppDbContext

# Aplicar migrations no banco de dados
dotnet ef database update \
  --project src/EvaAgent.Infra \
  --startup-project src/EvaAgent.Api \
  --context AppDbContext

# Reverter para uma migration específica
dotnet ef database update <NomeDaMigration> \
  --project src/EvaAgent.Infra \
  --startup-project src/EvaAgent.Api \
  --context AppDbContext

# Remover última migration (somente se não aplicada)
dotnet ef migrations remove \
  --project src/EvaAgent.Infra \
  --startup-project src/EvaAgent.Api
```

### Testes
```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura de código
dotnet test --collect:"XPlat Code Coverage"

# Executar testes de um projeto específico
dotnet test tests/EvaAgent.Dominio.Tests

# Gerar relatório de cobertura (requer ferramenta reportgenerator)
reportgenerator \
  "-reports:**/coverage.cobertura.xml" \
  "-targetdir:coverage-report" \
  "-reporttypes:Html"
```

### Infraestrutura Docker
```bash
# Iniciar todos os serviços (PostgreSQL, Redis, Seq, Jaeger)
cd deploy && docker-compose up -d

# Parar todos os serviços
cd deploy && docker-compose down

# Visualizar logs
cd deploy && docker-compose logs -f

# Rebuild e iniciar
cd deploy && docker-compose up -d --build
```

### Script de Início Rápido
```bash
# Usar o script de inicialização fornecido (inicia Docker + migrations + API)
./start.sh     # Linux/Mac
start.bat      # Windows
```

## Padrões Arquiteturais Principais

### 1. Repository Pattern
- `IRepositorioBase<T>` genérico fornece operações CRUD
- Implementado em `RepositorioBase<T>` usando EF Core
- Todos os repositórios usam soft delete (verificação do campo `ativo`)
- Exemplo de uso:
  ```csharp
  var usuarios = await _repositorio.BuscarAsync(u => u.Email.Contains("@example.com"));
  ```

### 2. Separação de Camadas de Serviço
- **Serviços de Domínio**: Lógica de negócio em `EvaAgent.Dominio/Interfaces/Servicos`
- **Serviços de Aplicação**: Orquestração em `EvaAgent.Aplicacao/Services`
- **Serviços de Infraestrutura**: Integrações externas em `EvaAgent.Infra/Servicos`

### 3. Strategy Pattern (Agentes)
- Interface `IAgenteEspecialista` para todos os agentes especializados
- Cada agente implementa:
  - `ProcessarMensagemAsync()`: Lógica principal de processamento
  - `PodeProcessarAsync()`: Validação de intenção
- Registrados no container de DI e resolvidos em tempo de execução

### 4. Factory Pattern (Provedores de IA & Conexões)
- `ProvedorIAFactory` cria instâncias de provedores de IA baseado na configuração
- `DbConnectionFactory` cria conexões de banco de dados baseado no tipo

## Convenções de Banco de Dados

### Nomenclatura
- **Tabelas**: snake_case (ex: `usuario`, `espaco`, `registro_pseudonimizacao`)
- **Colunas**: snake_case (ex: `nome`, `email_verificado`, `criado_em`)
- **Chaves Estrangeiras**: `{tabela}_id` (ex: `usuario_id`, `espaco_id`)
- **Índices**: `idx_{tabela}_{coluna}` (ex: `idx_usuario_email`)

### Configurações do Entity Framework
- Todas as entidades possuem configurações Fluent API em `EvaAgent.Infra/Data/Configurations/`
- Nomenclatura snake_case é forçada via `AppDbContext.ConfigureConventions()`
- Todas as entidades herdam de `EntidadeBase` (fornece `Id`, `CriadoEm`, `AtualizadoEm`, `Ativo`)

### Soft Delete
- Nunca use `DELETE` diretamente - sempre defina `Ativo = false`
- Repository base inclui método `DesativarAsync()`
- Todas as consultas automaticamente filtram `WHERE ativo = true` via global query filters

## Implementações Importantes de Serviços

### PseudonimizadorService (LGPD)
```csharp
// Antes de enviar para LLM:
var (textoPseudonimizado, registros) = await _pseudonimizador.PseudonimizarAsync(textoOriginal, conversaId);

// Após receber resposta da LLM:
var textoOriginal = await _pseudonimizador.ReverterPseudonimizacaoAsync(respostaIA, conversaId);
```

### IntentResolverService
```csharp
// Resolver qual agente deve tratar a mensagem:
var (agente, confianca) = await _intentResolver.ResolverIntencaoAsync(mensagem, espacoId);
if (confianca < 0.6m) {
    // Usar agente geral de fallback
}
```

### ConsultaExecutorDapper
```csharp
// Executar consulta dinâmica em banco de dados legado:
var parametros = new Dictionary<string, object> {
    { "dataInicio", DateTime.Today },
    { "dataFim", DateTime.Today.AddDays(1) }
};
var resultado = await _executor.ExecutarAsync(consultaId, parametros);
```

## Arquivos de Configuração

### appsettings.json
- **ConnectionStrings:DefaultConnection**: String de conexão PostgreSQL
- **Crypto:Key** e **Crypto:IV**: Chaves de criptografia AES-256 para dados sensíveis (DEVEM ser alteradas em produção)
- **OpenAI:ApiKey**, **Anthropic:ApiKey**: Chaves de API para provedores de LLM
- **Serilog**: Configuração de logging estruturado (Console + Seq)

### Importante: Segurança
- Nunca commite chaves de API ou chaves de criptografia no controle de versão
- Use variáveis de ambiente ou Azure Key Vault em produção
- Rotacione chaves de criptografia regularmente e re-criptografe os dados

## Diretrizes de Testes

### Estrutura de Testes
```
tests/
├── EvaAgent.Dominio.Tests/      # Testes unitários (≥ 90% cobertura)
├── EvaAgent.Aplicacao.Tests/    # Testes unitários (≥ 85% cobertura)
├── EvaAgent.Infra.Tests/        # Testes de integração (≥ 70% cobertura)
└── EvaAgent.Api.Tests/          # Testes E2E (≥ 75% cobertura)
```

### Ferramentas de Teste
- **xUnit**: Framework de testes
- **Moq**: Biblioteca de mocking
- **FluentAssertions**: Assertions legíveis (preferir ao invés de Assert.*)
- **Microsoft.AspNetCore.Mvc.Testing**: Testes de API
- **Testcontainers**: Containers Docker para testes de integração

### Comandos de Teste
```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório de cobertura
reportgenerator \
  "-reports:**/coverage.cobertura.xml" \
  "-targetdir:coverage-report" \
  "-reporttypes:Html"

# Executar testes específicos
dotnet test tests/EvaAgent.Dominio.Tests
```

### Padrões de Teste

#### Nomenclatura
```csharp
// Padrão: Deve<Ação><Condição>
[Fact]
public void DeveCriptografarTextoCorretamente() { }

[Fact]
public void DeveRetornarErro_QuandoTextoVazio() { }
```

#### Organização AAA
```csharp
[Fact]
public void TestExample()
{
    // Arrange - Preparar dados
    var service = new MinhaService();

    // Act - Executar ação
    var resultado = service.Processar("input");

    // Assert - Verificar resultado
    resultado.Should().Be("esperado");
}
```

#### Uso de Mocks
```csharp
var mock = new Mock<IRepositorio>();
mock.Setup(x => x.Obter(It.IsAny<Guid>()))
    .ReturnsAsync(new Entidade());
mock.Verify(x => x.Salvar(It.IsAny<Entidade>()), Times.Once);
```

### Metas de Cobertura
- **Domínio**: ≥ 90% (lógica de negócio pura)
- **Aplicação**: ≥ 85% (casos de uso)
- **Infraestrutura**: ≥ 70% (integrações)
- **API**: ≥ 75% (controllers)
- **Geral**: ≥ 80%

### Exemplos de Testes Criados
- `CryptoServiceTests.cs`: Testes de criptografia e hashing
- `UsuarioTests.cs`: Testes de entidade de domínio
- `PseudonimizadorServiceTests.cs`: Testes de pseudonimização LGPD
- `HealthControllerTests.cs`: Testes E2E de health checks
- `OrquestradorMensagensServiceTests.cs`: Testes de orquestração

Ver `tests/README.md` para documentação completa de testes.

## Estilo de Código e Convenções

### Linguagem
- **Código**: Todo código (classes, métodos, variáveis) em Português (pt-BR)
- **Comentários**: Português preferido, Inglês aceitável
- **Mensagens de Commit**: Português

### Formatação
- **Indentação**: 4 espaços (sem tabs)
- **Finais de Linha**: LF (estilo Unix)
- **Codificação**: UTF-8
- **Comprimento Máximo de Linha**: 120 caracteres (preferir legibilidade)

### Nomenclatura
- **Classes**: PascalCase (ex: `UsuarioConfiguration`, `PseudonimizadorService`)
- **Métodos**: PascalCase (ex: `ProcessarMensagemAsync`, `ObterPorIdAsync`)
- **Variáveis**: camelCase (ex: `espacoId`, `textoOriginal`)
- **Constantes**: UPPER_CASE (ex: `MAX_TENTATIVAS`)
- **Métodos Assíncronos**: Sempre com sufixo `Async`

## Entidades de Domínio Principais

### Entidades Core
- **Usuario**: Usuários do sistema
- **Espaco**: Espaços multi-tenant (hierárquicos)
- **Papel**: Papéis para RBAC
- **UsuarioEspaco**: Junção usuário-espaço-papel
- **Canal**: Canais de comunicação (WhatsApp, Telegram, etc.)
- **Receptor**: Receptores finais (clientes/usuários recebendo mensagens)
- **ProvedorIA**: Configurações de provedores de IA (OpenAI, Claude, Gemini)
- **Agente**: Agentes especializados para diferentes domínios de negócio
- **Conversa**: Threads de conversação
- **Mensagem**: Mensagens individuais

### Entidades LGPD
- **RegistroPseudonimizacao**: Rastreia dados pseudonimizados (GUID ↔ original criptografado)
- **ConsentimentoLGPD**: Gestão de consentimento do usuário

### Entidades de Lógica de Negócio
- **Sistema**: Sistemas legados para integrar
- **Conector**: Conectores de banco de dados ou API
- **ConsultaNegocio**: Consultas SQL parametrizadas
- **AcaoNegocio**: Ações para executar (POST/PUT/DELETE)

### Entidades de Memória
- **MemoriaCurtoPrazo**: Memória de curto prazo (escopo de conversa)
- **MemoriaLongoPrazo**: Memória de longo prazo (escopo de receptor)
- **GrupoTreinamento**: Grupos de documentos RAG
- **DocumentoTreinamento**: Documentos RAG com embeddings

## Workflows Comuns

### Adicionar uma Nova Entidade
1. Criar classe de entidade em `EvaAgent.Dominio/Entidades/`
2. Adicionar DbSet ao `AppDbContext`
3. Criar configuração Fluent API em `EvaAgent.Infra/Data/Configurations/`
4. Criar migration: `dotnet ef migrations add Add<Entidade>`
5. Aplicar migration: `dotnet ef database update`

### Adicionar um Novo Agente Especializado
1. Implementar `IAgenteEspecialista` em `EvaAgent.Infra/Servicos/Agentes/`
2. Registrar no container de DI no `Program.cs`
3. Adicionar configuração do agente aos seeds do banco de dados
4. Definir palavras-chave no campo `palavras_chave_json`

### Adicionar um Novo Provedor de IA
1. Implementar `IProvedorIA` em `EvaAgent.Infra/Servicos/IA/`
2. Atualizar `ProvedorIAFactory` para lidar com o novo tipo de provedor
3. Adicionar valor ao enum `TipoProvedor`
4. Atualizar seeds do banco de dados com nova opção de provedor

## Observabilidade

### Logging
- Logging estruturado via Serilog
- Console sink para desenvolvimento
- Seq sink para logging centralizado (http://localhost:5341)
- Níveis de log: Information, Warning, Error, Critical

### Tracing
- Jaeger para distributed tracing (http://localhost:16686)
- Rastreamento de fluxos de conversação ponta a ponta
- Correlação de requisições com GUID `codigo_correlacao`

### Métricas
- Métricas customizadas armazenadas na tabela `metrica_uso`
- Rastrear: contagem de mensagens, requisições de IA, custos, tempos de execução
- Agregar por espaço, data, tipo de recurso

### Auditoria
- Todas as operações registradas em `registro_auditoria`
- Captura: quem (usuario_id), o quê (entidade), quando (executado_em), mudanças (valores_antigos/novos)
- Endereço IP e user agent rastreados

## Considerações de Performance

### Banco de Dados
- Índices em chaves estrangeiras e colunas consultadas frequentemente
- Usar `AsNoTracking()` para consultas somente leitura
- Operações em lote quando possível

### Caching
- Redis para caching distribuído (ainda não implementado)
- Considerar cache de: respostas de IA, dados acessados frequentemente, tabelas de lookup

### Rate Limiting
- Rate limiting token bucket para webhooks (100 requisições/min)
- Limites por provedor para chamadas de IA

## Solução de Problemas

### Problemas com Migrations
- Se migration falhar, verifique a connection string no `appsettings.json`
- Garantir que PostgreSQL está rodando: `docker-compose ps`
- Verificar logs: `docker-compose logs postgres`

### API Não Está Iniciando
- Verificar se `appsettings.json` é JSON válido
- Verificar se porta 5000/5001 já está em uso
- Revisar logs da aplicação para erros de inicialização

### Erros de Conexão com Banco de Dados
- Verificar se PostgreSQL está acessível: `docker-compose exec postgres psql -U postgres -d evaagent`
- Verificar regras de firewall
- Validar formato da connection string

## Recursos

- **README do Projeto**: Documentação abrangente de arquitetura e banco de dados
- **Guia de Implementação**: Instruções passo a passo de implementação
- **Status do Projeto**: Status atual de implementação e próximos passos
- **Doc de Finalização**: Lista completa de funcionalidades e checklist de prontidão para produção
- **SWAGGER.md**: Documentação completa do Swagger/OpenAPI e como usá-lo
- **Swagger UI**: Interface interativa em `/swagger` para explorar e testar a API

## Notas Importantes para Claude Code

1. **Sempre use migrations** para mudanças no schema do banco - nunca modifique tabelas diretamente
2. **Respeite soft delete** - use `DesativarAsync()` ao invés de hard deletes
3. **Pseudonimize antes de LLM** - use PseudonimizadorService para qualquer dado contendo PII
4. **Escopo por EspacoId** - garantir que todas as consultas filtrem por tenant (espaço)
5. **Cobertura de testes importa** - manter ≥ 80% de cobertura geral
6. **Segurança primeiro** - nunca exponha chaves de criptografia, valide todas as entradas, use consultas parametrizadas
7. **Nomenclatura em português** - todos os elementos de código devem usar nomes em português
8. **Async até o fim** - use async/await para todas as operações de I/O
