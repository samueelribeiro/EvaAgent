# Agente Multissistema - Plataforma Orquestradora de IA

[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Coverage](https://img.shields.io/badge/coverage-%E2%89%A5%2080%25-brightgreen)](tests/)

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Arquitetura](#-arquitetura)
- [Banco de Dados](#-banco-de-dados)
- [Componentes Principais](#-componentes-principais)
- [Configuração e Execução](#-configuração-e-execução)
- [Testes](#-testes)
- [Endpoints da API](#-endpoints-da-api)
- [Fluxo de Processamento](#-fluxo-de-processamento)
- [Segurança e LGPD](#-segurança-e-lgpd)
- [Extensibilidade](#-extensibilidade)

## 🎯 Visão Geral

Esta plataforma é um **orquestrador inteligente de IA** que:

- Recebe mensagens de múltiplos canais (WhatsApp, Telegram, WebChat, Email, SMS)
- Identifica a intenção usando NLP e roteamento baseado em palavras-chave
- Delega para **agentes especialistas** por domínio de negócio
- Executa consultas (Dapper) e ações (API/DB) em sistemas legados
- **Pseudonimiza dados sensíveis** (LGPD) antes de enviar para LLMs
- Suporta múltiplos provedores de IA (OpenAI, Anthropic, Google)
- Mantém memória de curto e longo prazo
- Renderiza respostas com tom personalizado por receptor

### Casos de Uso

```
Usuário: "Qual foi o valor das vendas do dia de hoje?"
Sistema: [Resolve intenção] → Agente Financeiro → Consulta DB → IA formata → Resposta

Usuário: "Realize um lançamento no contas a receber no valor de 100 reais para o cliente José"
Sistema: [Resolve intenção] → Agente Financeiro → [Pseudonimiza "José"] → IA valida parâmetros
        → Executa Ação → [Reverte pseudonimização] → Confirma

Usuário: "Quantas vendas ocorreram hoje?"
Sistema: Agente Financeiro → Consulta DB → IA resume → Resposta com tom personalizado
```

## 🏗️ Arquitetura

### Camadas (Clean Architecture + DDD)

```
┌─────────────────────────────────────────────────────────┐
│                    API (ASP.NET Core)                    │
│  Controllers, Middlewares, HealthChecks, Auth (JWT)     │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                     Aplicação                            │
│  Services, DTOs, Casos de Uso, Orquestração             │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                      Domínio                             │
│  Entidades, ValueObjects, Interfaces, Regras Negócio    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                   Infraestrutura                         │
│  EF Core (PostgreSQL), Dapper (Multi-DB), HTTP Clients  │
│  Provedores IA, Crypto, Fila, Jobs                      │
└─────────────────────────────────────────────────────────┘
```

### Diagrama de Componentes

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│  WhatsApp    │────▶│  Webhook     │────▶│ Intent       │
│  Telegram    │     │  Controller  │     │ Resolver     │
│  WebChat     │     └──────────────┘     └──────┬───────┘
└──────────────┘                                  │
                                                  ▼
                           ┌──────────────────────────────────┐
                           │    Agente Roteador (Strategy)    │
                           └──────────┬───────────────────────┘
                                      │
        ┌─────────────────────────────┼─────────────────────────────┐
        ▼                             ▼                             ▼
┌───────────────┐           ┌───────────────┐            ┌───────────────┐
│ Agente        │           │ Agente        │            │ Agente        │
│ Multiprop.    │           │ Financeiro    │            │ Hotelaria     │
└───────┬───────┘           └───────┬───────┘            └───────┬───────┘
        │                           │                            │
        └───────────────────────────┼────────────────────────────┘
                                    ▼
                          ┌──────────────────┐
                          │ Pseudonimizador  │ (LGPD)
                          │ CPF/CNPJ → GUID  │
                          └────────┬─────────┘
                                   ▼
                     ┌──────────────────────────┐
                     │  Provedor IA Factory     │
                     │ (OpenAI/Claude/Gemini)   │
                     └────────┬─────────────────┘
                              ▼
                   ┌────────────────────────────┐
                   │ Executor (Dapper/API)      │
                   │ SQL Server/PostgreSQL/     │
                   │ Oracle/REST APIs           │
                   └────────────────────────────┘
```

## 🗄️ Banco de Dados

### Estrutura (PostgreSQL via EF Core)

#### Módulo: Identidade e Espaços

```sql
-- Usuários
usuario
├── id (PK)
├── nome
├── email (unique)
├── senha_hash
├── telefone
├── avatar
├── timezone
├── idioma
├── ultimo_acesso
├── email_verificado
├── criado_em
├── atualizado_em
└── ativo

-- Espaços (multi-tenancy)
espaco
├── id (PK)
├── nome
├── descricao
├── slug (unique)
├── logo_url
├── espaco_pai_id (FK → espaco) -- Hierarquia
├── criado_em
├── atualizado_em
└── ativo

-- Papéis (RBAC)
papel
├── id (PK)
├── nome
├── descricao
├── tipo (Administrador, Membro)
├── permissoes (JSONB)
├── criado_em
└── ativo

-- Vínculo Usuário-Espaço-Papel
usuario_espaco
├── id (PK)
├── usuario_id (FK → usuario)
├── espaco_id (FK → espaco)
├── papel_id (FK → papel)
├── convidado_em
├── aceito_em
├── criado_em
└── ativo
```

#### Módulo: Canais e Receptores

```sql
-- Canais de Comunicação
canal
├── id (PK)
├── espaco_id (FK → espaco)
├── nome
├── tipo (WhatsApp, Telegram, WebChat, Email, SMS, Voz)
├── configuracao_json (JSONB) -- tokens, webhooks, credenciais
├── habilitado
├── criado_em
└── ativo

-- Receptores (destinatários finais)
receptor
├── id (PK)
├── canal_id (FK → canal)
├── identificador (telefone, email, user_id)
├── nome
├── email
├── avatar
├── metadados_json (JSONB)
-- Preferências de Tom
├── tom_atendimento (Informal, Formal, Amigavel, Profissional)
├── formato_nome (PrimeiroNome, NomeCompleto, Sobrenome, Apelido)
├── usar_saudacao
├── idioma
├── timezone
├── criado_em
└── ativo
```

#### Módulo: Provedores de IA

```sql
-- Configuração de LLMs
provedor_ia
├── id (PK)
├── espaco_id (FK → espaco)
├── nome
├── tipo (OpenAI, Anthropic, Google)
├── url_base
├── chave_api (cifrada AES-256)
├── modelo (gpt-4, claude-3-opus, gemini-pro)
├── max_tokens
├── temperatura
├── habilitado
├── limite_requisicoes_por_minuto
├── criado_em
└── ativo

-- Log de Solicitações
solicitacao_ia
├── id (PK)
├── provedor_ia_id (FK → provedor_ia)
├── conversa_id (FK → conversa) [nullable]
├── prompt (text)
├── contexto_json (JSONB) -- histórico, memória
├── tokens_prompt
├── solicitado_em
├── criado_em
└── ativo

-- Log de Respostas
resposta_ia
├── id (PK)
├── solicitacao_ia_id (FK → solicitacao_ia)
├── resposta (text)
├── tokens_resposta
├── custo_estimado
├── tempo_resposta_ms
├── respondido_em
├── metadados_json (JSONB)
├── criado_em
└── ativo
```

#### Módulo: Agentes Especialistas

```sql
-- Agentes
agente
├── id (PK)
├── espaco_id (FK → espaco)
├── nome
├── descricao
├── tipo (Multipropriedade, Financeiro, Hotelaria, Gestao, Administradores, Geral)
├── prompt_sistema (text)
├── palavras_chave_json (JSONB) -- ["venda", "financeiro", "boleto"]
├── habilitado
├── prioridade (int) -- para desempate
├── criado_em
└── ativo

-- Intenções por Agente
intencao_agente
├── id (PK)
├── agente_id (FK → agente)
├── nome
├── descricao
├── palavras_chave_json (JSONB)
├── exemplos_json (JSONB)
├── confianca (decimal) -- threshold 0.0-1.0
├── criado_em
└── ativo
```

#### Módulo: Conectores e Sistemas

```sql
-- Sistemas Legados
sistema
├── id (PK)
├── espaco_id (FK → espaco)
├── nome
├── descricao
├── versao_api
├── criado_em
└── ativo

-- Conectores (API ou DB)
conector
├── id (PK)
├── sistema_id (FK → sistema)
├── nome
├── tipo (ApiRest, BancoDados, Arquivo)
-- API REST
├── url_base
├── chave_api (cifrada)
├── headers_json (JSONB)
-- Banco de Dados
├── tipo_banco_dados (SqlServer, PostgreSQL, Oracle, MySQL)
├── string_conexao (cifrada)
├── habilitado
├── timeout_segundos
├── criado_em
└── ativo

-- Consultas de Negócio
consulta_negocio
├── id (PK)
├── sistema_id (FK → sistema)
├── nome
├── descricao
├── query_sql (text)
├── parametros_json (JSONB) -- schema
├── requer_autenticacao
├── criado_em
└── ativo

-- Ações de Negócio
acao_negocio
├── id (PK)
├── sistema_id (FK → sistema)
├── nome
├── descricao
├── endpoint_url
├── metodo_http (POST, PUT, DELETE)
├── script_sql (text) -- para ações em DB
├── parametros_json (JSONB)
├── requer_confirmacao
├── requer_autenticacao
├── criado_em
└── ativo
```

#### Módulo: Conversas e Mensagens

```sql
-- Conversas
conversa
├── id (PK)
├── receptor_id (FK → receptor)
├── agente_id (FK → agente) [nullable]
├── titulo
├── iniciada_em
├── finalizada_em
├── resumo_json (JSONB)
├── arquivada
├── criado_em
└── ativo

-- Mensagens
mensagem
├── id (PK)
├── conversa_id (FK → conversa)
├── direcao (Entrada, Saida)
├── conteudo (text)
├── status (Recebida, Processando, Enviada, Entregue, Lida, Erro)
├── enviada_em
├── entregue_em
├── lida_em
├── midia_url
├── tipo_midia
├── metadados_json (JSONB)
├── id_externo -- ID do canal externo (WhatsApp message_id, etc.)
├── criado_em
└── ativo
```

#### Módulo: LGPD (Pseudonimização)

```sql
-- Registro de Pseudonimização
registro_pseudonimizacao
├── id (PK)
├── guid (unique) -- GUID gerado para substituir dado original
├── valor_original_hash (SHA-256)
├── valor_cifrado (AES-256)
├── tipo_dado (CPF, CNPJ, Nome, Email, Telefone)
├── conversa_id (FK → conversa) [nullable]
├── solicitacao_ia_id (FK → solicitacao_ia) [nullable]
├── pseudonimizado_em
├── revertido_em
├── expira_em -- para limpeza automática
├── criado_em
└── ativo

-- Consentimentos LGPD
consentimento_lgpd
├── id (PK)
├── receptor_id (FK → receptor)
├── finalidade
├── consentido (boolean)
├── consentido_em
├── revogado_em
├── ip_origem
├── criado_em
└── ativo
```

#### Módulo: Memória e RAG

```sql
-- Memória de Curto Prazo (escopo conversa)
memoria_curto_prazo
├── id (PK)
├── conversa_id (FK → conversa)
├── chave
├── valor (text)
├── expira_em
├── criado_em
└── ativo

-- Memória de Longo Prazo (escopo receptor)
memoria_longo_prazo
├── id (PK)
├── receptor_id (FK → receptor)
├── chave
├── valor (text)
├── categoria
├── importancia_score
├── criado_em
├── atualizado_em
└── ativo

-- Grupos de Treinamento (RAG)
grupo_treinamento
├── id (PK)
├── espaco_id (FK → espaco)
├── nome
├── descricao
├── tags (CSV)
├── criado_em
└── ativo

-- Documentos de Treinamento
documento_treinamento
├── id (PK)
├── grupo_treinamento_id (FK → grupo_treinamento)
├── nome
├── descricao
├── conteudo_original (text)
├── conteudo_processado (text)
├── embedding (vector) -- pgvector para busca semântica
├── metadados_json (JSONB)
├── ingerido_em
├── criado_em
└── ativo
```

#### Módulo: Orquestração e Fila

```sql
-- Tarefas Agendadas
tarefa_agendada
├── id (PK)
├── espaco_id (FK → espaco)
├── nome
├── tipo_tarefa
├── parametros_json (JSONB)
├── cron_expressao (string) -- "0 */5 * * * *"
├── proxima_execucao
├── ultima_execucao
├── status (Pendente, Processando, Concluida, Erro, Cancelada)
├── habilitada
├── criado_em
└── ativo

-- Fila de Mensagens
fila_mensagem
├── id (PK)
├── espaco_id (FK → espaco)
├── tipo_mensagem
├── conteudo_json (JSONB)
├── status (Recebida, Processando, Enviada, Erro)
├── tentativas_processamento
├── max_tentativas
├── processado_em
├── erro_mensagem
├── criado_em
└── ativo

-- Dead Letter Queue
fila_deadletter
├── id (PK)
├── fila_mensagem_id (FK → fila_mensagem)
├── tipo_mensagem
├── conteudo_json (JSONB)
├── erro_mensagem
├── stack_trace (text)
├── tentativas_processamento
├── enviado_deadletter_em
├── criado_em
└── ativo
```

#### Módulo: Observabilidade

```sql
-- Auditoria
registro_auditoria
├── id (PK)
├── usuario_id (FK → usuario) [nullable]
├── espaco_id (FK → espaco) [nullable]
├── entidade
├── acao (Create, Update, Delete, Read)
├── entidade_id
├── valores_antigos (JSONB)
├── valores_novos (JSONB)
├── ip_origem
├── user_agent
├── executado_em
├── criado_em
└── ativo

-- Erros
registro_erro
├── id (PK)
├── espaco_id (FK → espaco) [nullable]
├── codigo_correlacao (GUID)
├── tipo_erro
├── mensagem (text)
├── stack_trace (text)
├── contexto_json (JSONB)
├── severidade (1=Low, 2=Medium, 3=High, 4=Critical)
├── ocorrido_em
├── resolvido (boolean)
├── criado_em
└── ativo

-- Métricas de Uso
metrica_uso
├── id (PK)
├── espaco_id (FK → espaco)
├── tipo_metrica (mensagens, requisicoes_ia, execucoes, custos)
├── recurso
├── quantidade
├── unidade_medida
├── dimensoes_json (JSONB)
├── medido_em
├── criado_em
└── ativo

-- Execuções (Log de Consultas/Ações)
registro_execucao
├── id (PK)
├── espaco_id (FK → espaco)
├── conversa_id (FK → conversa) [nullable]
├── tipo_execucao (Consulta, Acao)
├── recurso_id (id da consulta ou ação)
├── nome_recurso
├── parametros_json (JSONB)
├── resultado_json (JSONB)
├── sucesso (boolean)
├── tempo_execucao_ms
├── executado_em
├── criado_em
└── ativo
```

### Índices Recomendados

```sql
-- Identidade
CREATE INDEX idx_usuario_email ON usuario(email);
CREATE INDEX idx_espaco_slug ON espaco(slug);
CREATE INDEX idx_usuario_espaco_lookup ON usuario_espaco(usuario_id, espaco_id);

-- Canais
CREATE INDEX idx_receptor_canal ON receptor(canal_id);
CREATE INDEX idx_receptor_identificador ON receptor(identificador);

-- Conversas
CREATE INDEX idx_conversa_receptor ON conversa(receptor_id);
CREATE INDEX idx_conversa_agente ON conversa(agente_id);
CREATE INDEX idx_mensagem_conversa ON mensagem(conversa_id);
CREATE INDEX idx_mensagem_status ON mensagem(status);
CREATE INDEX idx_mensagem_enviada_em ON mensagem(enviada_em DESC);

-- LGPD
CREATE UNIQUE INDEX idx_pseudonimizacao_guid ON registro_pseudonimizacao(guid);
CREATE INDEX idx_pseudonimizacao_conversa ON registro_pseudonimizacao(conversa_id);
CREATE INDEX idx_pseudonimizacao_solicitacao ON registro_pseudonimizacao(solicitacao_ia_id);
CREATE INDEX idx_pseudonimizacao_expira ON registro_pseudonimizacao(expira_em) WHERE ativo = true;

-- Memória
CREATE INDEX idx_memoria_cp_conversa ON memoria_curto_prazo(conversa_id, chave);
CREATE INDEX idx_memoria_lp_receptor ON memoria_longo_prazo(receptor_id, categoria);

-- Fila
CREATE INDEX idx_fila_status_criado ON fila_mensagem(status, criado_em) WHERE ativo = true;

-- Observabilidade
CREATE INDEX idx_auditoria_entidade ON registro_auditoria(entidade, entidade_id);
CREATE INDEX idx_erro_codigo_correlacao ON registro_erro(codigo_correlacao);
CREATE INDEX idx_execucao_conversa ON registro_execucao(conversa_id);
```

## 📦 Componentes Principais

### 1. Pseudonimização LGPD

**Fluxo**:
1. Texto de entrada: "Cliente José Silva, CPF 123.456.789-00"
2. Regex detecta: `Nome: José Silva`, `CPF: 123.456.789-00`
3. Gera GUIDs: `{guid-1}`, `{guid-2}`
4. Substitui: "Cliente {guid-1}, CPF {guid-2}"
5. Armazena em `registro_pseudonimizacao`:
   - `guid={guid-1}`, `valor_cifrado=AES("José Silva")`, `hash=SHA256("José Silva")`
6. Envia para IA: "Cliente {guid-1}, CPF {guid-2}"
7. IA responde: "O boleto de {guid-1} está vencido"
8. Reverte: "O boleto de José Silva está vencido"

**Implementação** (`EvaAgent.Infra/Servicos/LGPD`):
- `PseudonimizadorService.cs`
- `CryptoService.cs` (AES-256 + SHA-256)
- Middleware: `PseudonimizacaoMiddleware.cs`

### 2. Provedores de IA

**Factory Pattern**:
```csharp
IProvedorIA provedor = ProvedorIAFactory.Criar(provedorConfig);
var resposta = await provedor.GerarRespostaAsync(prompt, contexto);
```

**Implementações**:
- `OpenAIProvedor.cs`: OpenAI GPT-4/3.5 via HTTP Client
- `ClaudeProvedor.cs`: Anthropic Claude via API
- `GeminiProvedor.cs`: Google Gemini via Vertex AI

**Rate Limiting**: Implementado via `System.Threading.RateLimiting` (token bucket)

### 3. Roteamento de Intenções

**IntentResolverService.cs**:
```csharp
public async Task<(Agente? Agente, decimal Confianca)> ResolverIntencaoAsync(
    string mensagem,
    Guid espacoId)
{
    // 1. Tokeniza mensagem
    var tokens = Tokenizar(mensagem);

    // 2. Busca agentes do espaço
    var agentes = await _repositorio.BuscarPorEspacoAsync(espacoId);

    // 3. Calcula score para cada agente
    var scores = agentes.Select(a => new {
        Agente = a,
        Score = CalcularScore(tokens, a.PalavrasChave)
    });

    // 4. Retorna agente com maior score (se >= threshold)
    var melhor = scores.OrderByDescending(s => s.Score).FirstOrDefault();
    return melhor?.Score >= 0.6m
        ? (melhor.Agente, melhor.Score)
        : (null, 0);
}
```

### 4. Agentes Especialistas (Strategy Pattern)

**Interface**:
```csharp
public interface IAgenteEspecialista
{
    string Nome { get; }
    Task<string> ProcessarMensagemAsync(string mensagem, Guid conversaId);
    Task<bool> PodeProcessarAsync(string mensagem);
}
```

**Implementações**:
- `AgenteFinanceiro.cs`
- `AgenteMultipropriedade.cs`
- `AgenteHotelaria.cs`
- `AgenteGestao.cs`
- `AgenteAdministradores.cs`

### 5. Conectores e Executores

**Dapper Multi-SGBD** (`DbConnectionFactory.cs`):
```csharp
public IDbConnection CriarConexao(TipoBancoDados tipo, string connectionString)
{
    return tipo switch
    {
        TipoBancoDados.SqlServer => new SqlConnection(connectionString),
        TipoBancoDados.PostgreSQL => new NpgsqlConnection(connectionString),
        TipoBancoDados.Oracle => new OracleConnection(connectionString),
        TipoBancoDados.MySQL => new MySqlConnection(connectionString),
        _ => throw new NotSupportedException()
    };
}
```

**ConsultaExecutorDapper.cs**:
```csharp
public async Task<IEnumerable<dynamic>> ExecutarConsultaAsync(
    Guid consultaId,
    Dictionary<string, object> parametros)
{
    var consulta = await _repo.ObterPorIdAsync(consultaId);
    var conector = await _repoConector.ObterPorIdAsync(consulta.Sistema.ConectorId);

    using var conn = _factory.CriarConexao(
        conector.TipoBancoDados!.Value,
        _crypto.Descriptografar(conector.StringConexao));

    return await conn.QueryAsync(consulta.QuerySql, parametros);
}
```

### 6. Sistema de Memória

**Curto Prazo** (sessão/conversa):
- TTL configurável (ex.: 1 hora)
- Contexto da conversa atual
- Preferências temporárias

**Longo Prazo** (receptor):
- Informações permanentes
- Histórico relevante
- Aprendizados

**RAG (Retrieval-Augmented Generation)**:
- Ingestão de documentos
- Embeddings (pgvector ou Azure Cognitive Search)
- Busca semântica
- Enriquecimento de contexto

### 7. Fila e Jobs

**FilaService.cs**:
- Processamento assíncrono
- Retry com exponential backoff
- Dead Letter Queue para falhas irrecuperáveis

**TarefaSchedulerHostedService.cs** (Background Service):
```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        var tarefas = await _repo.ObterTarefasPendentesAsync();
        foreach (var tarefa in tarefas)
        {
            if (tarefa.DeveExecutar())
            {
                await _executor.ExecutarAsync(tarefa);
            }
        }
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
    }
}
```

### 8. Renderização de Tom

**RenderizadorTomService.cs**:
```csharp
public string RenderizarResposta(string resposta, Receptor receptor)
{
    var texto = resposta;

    // Aplicar formato de nome
    texto = AplicarFormatoNome(texto, receptor);

    // Aplicar tom (Informal → "Oi!", Formal → "Prezado Sr./Sra.")
    texto = AplicarTom(texto, receptor.TomAtendimento);

    // Traduzir se necessário
    if (!string.IsNullOrEmpty(receptor.Idioma))
    {
        texto = Traduzir(texto, receptor.Idioma);
    }

    // Ajustar timezone para datas/horas
    texto = AjustarTimezone(texto, receptor.TimeZone);

    return texto;
}
```

## ⚙️ Configuração e Execução

### Pré-requisitos

- .NET 9.0 SDK
- PostgreSQL 16+ (com extensão `pgvector` para RAG)
- Docker e Docker Compose (opcional, mas recomendado)
- Visual Studio 2022 ou VS Code + C# Dev Kit

### Variáveis de Ambiente

Crie um arquivo `.env` na raiz:

```bash
# Database
DATABASE_HOST=localhost
DATABASE_PORT=5432
DATABASE_NAME=evaagent
DATABASE_USER=postgres
DATABASE_PASSWORD=SuaSenhaSegura123!

# JWT
JWT_SECRET_KEY=sua-chave-secreta-super-segura-min-256-bits
JWT_ISSUER=https://evaagent.local
JWT_AUDIENCE=https://evaagent.local
JWT_EXPIRATION_MINUTES=60

# LGPD Crypto
CRYPTO_KEY=sua-chave-aes-256-base64==
CRYPTO_IV=seu-iv-aes-base64==

# OpenAI
OPENAI_API_KEY=sk-...
OPENAI_ORGANIZATION=org-...

# Anthropic
ANTHROPIC_API_KEY=sk-ant-...

# Google
GOOGLE_APPLICATION_CREDENTIALS=/path/to/service-account.json
GOOGLE_PROJECT_ID=seu-projeto-gcp

# WhatsApp Business API (exemplo)
WHATSAPP_API_URL=https://api.whatsapp.com/v1
WHATSAPP_API_TOKEN=seu-token

# Observabilidade
SERILOG_SEQ_URL=http://localhost:5341
APPLICATION_INSIGHTS_KEY=sua-chave-azure
```

### Executar com Docker Compose

```bash
# 1. Subir infraestrutura (PostgreSQL, Redis, Seq)
cd deploy
docker compose up -d

# 2. Aplicar migrations
cd ../src/EvaAgent.Api
dotnet ef database update --project ../EvaAgent.Infra

# 3. Executar a aplicação
dotnet run

# API estará disponível em https://localhost:7001
```

### Executar Manualmente

```bash
# 1. Restaurar pacotes
dotnet restore

# 2. Aplicar migrations
dotnet ef database update --project src/EvaAgent.Infra --startup-project src/EvaAgent.Api

# 3. Executar
dotnet run --project src/EvaAgent.Api
```

### Seeds Iniciais

O sistema cria automaticamente:
- **Papel Administrador** e **Papel Membro**
- **Usuário Admin** (admin@evaagent.local / Admin@123)
- **Espaço Padrão** ("Espaço Demo")
- **Provedores de IA** (desabilitados por padrão, configure suas chaves)

## 🧪 Testes

### Estrutura de Testes

```
tests/
├── EvaAgent.Dominio.Tests/        # Testes de unidade (regras de negócio)
│   ├── Entidades/
│   ├── ValueObjects/
│   └── Servicos/
├── EvaAgent.Aplicacao.Tests/      # Testes de unidade (casos de uso)
│   ├── Services/
│   ├── Validators/
│   └── Builders/                  # Builders para fixtures
├── EvaAgent.Infra.Tests/          # Testes de integração (DB, HTTP)
│   ├── Repositories/
│   ├── Servicos/
│   └── Fixtures/                  # Testcontainers, WebApplicationFactory
└── EvaAgent.Api.Tests/            # Testes E2E e de contrato
    ├── Controllers/
    ├── Middlewares/
    ├── E2E/
    └── Contracts/                 # Mocks de LLMs, APIs externas
```

### Executar Testes

```bash
# Todos os testes
dotnet test

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Relatório de cobertura (ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator "-reports:**/coverage.cobertura.xml" "-targetdir:coverage-report" "-reporttypes:Html"

# Abrir relatório
start coverage-report/index.html  # Windows
open coverage-report/index.html   # macOS
xdg-open coverage-report/index.html # Linux
```

### Metas de Cobertura

- **Domínio**: ≥ 90%
- **Aplicação**: ≥ 85%
- **Infraestrutura**: ≥ 70%
- **API**: ≥ 75%
- **Geral**: ≥ 80%

### Exemplos de Testes

**Unidade** (Domínio):
```csharp
[Fact]
public void Pseudonimizador_DeveCriptografarEDescriptografar()
{
    // Arrange
    var crypto = new CryptoService(key, iv);
    var original = "José Silva";

    // Act
    var cifrado = crypto.Criptografar(original);
    var decifrado = crypto.Descriptografar(cifrado);

    // Assert
    Assert.NotEqual(original, cifrado);
    Assert.Equal(original, decifrado);
}
```

**Integração** (EF Core):
```csharp
[Fact]
public async Task Repositorio_DeveSalvarERecuperarUsuario()
{
    // Arrange
    using var factory = new WebApplicationFactory<Program>();
    var scope = factory.Services.CreateScope();
    var repo = scope.ServiceProvider.GetRequiredService<IRepositorioBase<Usuario>>();

    // Act
    var usuario = new Usuario { Nome = "Teste", Email = "teste@exemplo.com" };
    await repo.AdicionarAsync(usuario);
    var recuperado = await repo.ObterPorIdAsync(usuario.Id);

    // Assert
    Assert.NotNull(recuperado);
    Assert.Equal("Teste", recuperado.Nome);
}
```

**Contrato** (LLM Mock):
```csharp
[Fact]
public async Task OpenAIProvedor_DeveRetornarRespostaValida()
{
    // Arrange
    var mockHandler = new MockHttpMessageHandler();
    mockHandler.When("https://api.openai.com/v1/chat/completions")
        .Respond("application/json", JsonSerializer.Serialize(new {
            choices = new[] {
                new { message = new { content = "Resposta mockada" } }
            }
        }));

    var cliente = new HttpClient(mockHandler);
    var provedor = new OpenAIProvedor(cliente, "fake-key");

    // Act
    var resposta = await provedor.GerarRespostaAsync("Teste");

    // Assert
    Assert.Equal("Resposta mockada", resposta);
}
```

**E2E** (Fluxo Completo):
```csharp
[Fact]
public async Task FluxoCompleto_WebhookParaResposta()
{
    // Arrange
    using var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();

    // Act: Enviar mensagem via webhook
    var payload = new {
        from = "+5511999999999",
        message = "Qual foi o valor das vendas de hoje?"
    };
    var response = await client.PostAsJsonAsync("/api/webhook/whatsapp", payload);

    // Assert: Verificar que retornou 200 e processou
    response.EnsureSuccessStatusCode();

    // Verificar que criou conversa
    var scope = factory.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var conversa = await dbContext.Conversas
        .Include(c => c.Mensagens)
        .FirstOrDefaultAsync(c => c.Receptor.Identificador == "+5511999999999");

    Assert.NotNull(conversa);
    Assert.Contains(conversa.Mensagens, m => m.Direcao == DirecaoMensagem.Entrada);
    Assert.Contains(conversa.Mensagens, m => m.Direcao == DirecaoMensagem.Saida);
}
```

## 🔌 Endpoints da API

### Autenticação

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@evaagent.local",
  "senha": "Admin@123"
}

→ 200 OK
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600,
  "usuario": { "id": "...", "nome": "Admin" }
}
```

### Webhooks

```http
POST /api/webhook/whatsapp
Content-Type: application/json
X-Hub-Signature: sha256=...

{
  "from": "+5511999999999",
  "message": "Qual o saldo do mês?",
  "timestamp": "2025-01-15T14:30:00Z"
}

→ 200 OK
{
  "status": "received",
  "conversationId": "guid..."
}
```

### Chat

```http
POST /api/chat
Authorization: Bearer {token}
Content-Type: application/json

{
  "receptorId": "guid...",
  "mensagem": "Quantas vendas ocorreram hoje?"
}

→ 200 OK
{
  "conversaId": "guid...",
  "mensagemId": "guid...",
  "resposta": "Hoje ocorreram 15 vendas, totalizando R$ 12.500,00.",
  "agenteUtilizado": "Agente Financeiro",
  "confianca": 0.92
}
```

### Consultas

```http
POST /api/consultas/vendas-do-dia:executar
Authorization: Bearer {token}
Content-Type: application/json

{
  "dataInicio": "2025-01-15T00:00:00Z",
  "dataFim": "2025-01-15T23:59:59Z"
}

→ 200 OK
{
  "resultado": [
    { "id": 1, "valor": 150.00, "cliente": "João" },
    { "id": 2, "valor": 300.00, "cliente": "Maria" }
  ],
  "total": 450.00,
  "tempoExecucao": 245
}
```

### Ações

```http
POST /api/acoes/lancar-contas-receber:executar
Authorization: Bearer {token}
Content-Type: application/json

{
  "clienteId": "guid...",
  "valor": 100.00,
  "descricao": "Serviço de consultoria",
  "vencimento": "2025-02-15"
}

→ 200 OK
{
  "sucesso": true,
  "lancamentoId": "guid...",
  "mensagem": "Lançamento realizado com sucesso"
}
```

### Treinamento (RAG)

```http
POST /api/treinamento/documentos:ingestar
Authorization: Bearer {token}
Content-Type: application/json

{
  "grupoTreinamentoId": "guid...",
  "nome": "Manual de Vendas v2.0",
  "conteudo": "Este documento descreve o processo de vendas..."
}

→ 201 Created
{
  "documentoId": "guid...",
  "embedding": true,
  "chunks": 12
}
```

### Conversas

```http
GET /api/conversas/{id}
Authorization: Bearer {token}

→ 200 OK
{
  "id": "guid...",
  "receptor": { "nome": "João Silva", "telefone": "+5511..." },
  "agente": { "nome": "Agente Financeiro" },
  "iniciada": "2025-01-15T14:00:00Z",
  "mensagens": [
    {
      "direcao": "Entrada",
      "conteudo": "Qual meu saldo?",
      "enviada": "2025-01-15T14:00:10Z"
    },
    {
      "direcao": "Saida",
      "conteudo": "Seu saldo atual é R$ 1.250,00.",
      "enviada": "2025-01-15T14:00:15Z"
    }
  ]
}
```

### Administração

```http
# Criar Espaço
POST /api/admin/espacos
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Empresa XYZ",
  "descricao": "Espaço da empresa XYZ"
}

# Criar Usuário
POST /api/admin/usuarios
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Maria Santos",
  "email": "maria@exemplo.com",
  "senha": "Senha@123",
  "espacoId": "guid...",
  "papelId": "guid..."
}

# Configurar Provedor IA
POST /api/admin/credenciais-ia
Authorization: Bearer {token}
Content-Type: application/json

{
  "espacoId": "guid...",
  "tipo": "OpenAI",
  "chaveApi": "sk-...",
  "modelo": "gpt-4",
  "maxTokens": 2000,
  "temperatura": 0.7
}
```

## 🔄 Fluxo de Processamento

### Fluxo Completo (Exemplo: WhatsApp)

```
┌─────────────────┐
│ 1. Webhook      │ Recebe: from=+5511..., message="Qual o saldo?"
│ WhatsApp        │
└────────┬────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 2. WebhookController                                        │
│  - Valida assinatura (X-Hub-Signature)                     │
│  - Registra mensagem (Entrada, Status=Recebida)            │
│  - Identifica/cria Receptor                                │
│  - Cria/recupera Conversa                                  │
└────────┬───────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 3. ConversacaoService                                       │
│  - Busca Espaço do Receptor                                │
│  - Carrega Memória (curto + longo prazo)                   │
│  - Chama IntentResolverService                             │
└────────┬───────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 4. IntentResolverService                                    │
│  - Tokeniza: ["qual", "saldo"]                             │
│  - Busca Agentes do Espaço                                 │
│  - Calcula score:                                          │
│    * Agente Financeiro: 0.92 (match: "saldo")              │
│    * Agente Hotelaria: 0.15                                │
│  - Retorna: (Agente Financeiro, 0.92)                      │
└────────┬───────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 5. AgenteFinanceiro (IAgenteEspecialista)                   │
│  - Extrai parâmetros: { receptorId: "..." }                │
│  - Identifica ação: ConsultarSaldo                         │
│  - Chama ConsultaExecutorDapper                            │
└────────┬───────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 6. ConsultaExecutorDapper                                   │
│  - Busca ConsultaNegocio "saldo-cliente"                   │
│  - Recupera Conector do Sistema                            │
│  - Descriptografa StringConexao                            │
│  - Executa: SELECT saldo FROM contas WHERE cliente=@id     │
│  - Retorna: { saldo: 1250.00 }                             │
└────────┬───────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 7. Pseudonimização (se houver dados sensíveis)             │
│  - Texto: "O saldo de José Silva é R$ 1.250,00"            │
│  - Detecta: "José Silva"                                   │
│  - Gera GUID: {guid-1}                                     │
│  - Cifra e armazena: registro_pseudonimizacao              │
│  - Texto pseudonimizado: "O saldo de {guid-1} é..."        │
└────────┬───────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 8. Provedor IA (OpenAI/Claude/Gemini)                       │
│  - Contexto: Memória + Histórico conversa                  │
│  - Prompt: "Responda de forma amigável sobre saldo"        │
│  - Dados: { saldo: 1250.00 }                               │
│  - LLM gera: "Seu saldo atual é R$ 1.250,00."              │
│  - Registra: solicitacao_ia, resposta_ia                   │
└────────┬───────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 9. Reversão Pseudonimização                                 │
│  - Texto IA: "O saldo de {guid-1} é R$ 1.250,00"           │
│  - Busca {guid-1} → Decifra → "José Silva"                 │
│  - Texto final: "O saldo de José Silva é R$ 1.250,00"      │
└────────┬───────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 10. Renderizador de Tom                                     │
│  - Busca preferências do Receptor:                         │
│    * Tom: Informal                                         │
│    * Formato nome: PrimeiroNome                            │
│    * Saudação: true                                        │
│  - Aplica: "Oi João! Seu saldo atual é R$ 1.250,00."      │
└────────┬───────────────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────────────┐
│ 11. Resposta                                                │
│  - Registra Mensagem (Saida, Status=Enviada)              │
│  - Envia via Canal (WhatsApp API)                         │
│  - Atualiza Status: Entregue → Lida                        │
│  - Registra Métricas e Auditoria                           │
└─────────────────────────────────────────────────────────────┘
```

## 🔒 Segurança e LGPD

### Criptografia

- **Dados em trânsito**: TLS 1.3
- **Dados em repouso**:
  - `chave_api`: AES-256-GCM
  - `string_conexao`: AES-256-GCM
  - `senha_hash`: BCrypt (work factor 12)
- **Hashing**: SHA-256 para pseudonimização

### Autenticação e Autorização

- **JWT**: HS256 (HMAC-SHA256)
- **Claims**: `sub` (usuário ID), `espaco_id`, `role`
- **Policies**:
  ```csharp
  [Authorize(Policy = "RequireAdministradorRole")]
  [Authorize(Policy = "RequireEspacoAccess")]
  ```

### LGPD - Conformidade

1. **Pseudonimização Automática**:
   - Antes de enviar para LLM externo
   - Padrões detectados: CPF, CNPJ, Email, Telefone, Nomes próprios (NER)

2. **Consentimento**:
   - Tabela `consentimento_lgpd`
   - Verificação antes de processar dados pessoais

3. **Direito ao Esquecimento**:
   - Soft delete (campo `ativo`)
   - Hard delete após período legal
   - Anonimização de dados históricos

4. **Auditoria Completa**:
   - Quem acessou (usuário_id)
   - Quando (executado_em)
   - O quê (entidade, entidade_id)
   - Como (valores_antigos, valores_novos)

5. **Expiração de Dados**:
   - `registro_pseudonimizacao.expira_em`
   - Job limpa registros expirados

### Rate Limiting

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("webhook", opt =>
    {
        opt.TokenLimit = 100;
        opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
        opt.TokensPerPeriod = 20;
    });
});

app.MapPost("/api/webhook/whatsapp", ...)
   .RequireRateLimiting("webhook");
```

## 🚀 Extensibilidade

### Adicionar Novo Canal

1. Criar enum em `TipoCanal.cs`:
   ```csharp
   public enum TipoCanal
   {
       // ...
       Discord = 7
   }
   ```

2. Criar `DiscordController.cs`:
   ```csharp
   [ApiController]
   [Route("api/webhook/discord")]
   public class DiscordController : ControllerBase
   {
       [HttpPost]
       public async Task<IActionResult> ReceberMensagem([FromBody] DiscordWebhookPayload payload)
       {
           // Processar e delegar para ConversacaoService
       }
   }
   ```

3. Configurar Canal no admin

### Adicionar Novo Agente Especialista

1. Criar `AgenteLogistica.cs`:
   ```csharp
   public class AgenteLogistica : IAgenteEspecialista
   {
       public string Nome => "Agente Logística";

       public async Task<string> ProcessarMensagemAsync(string mensagem, Guid conversaId)
       {
           // Lógica específica de logística
       }

       public async Task<bool> PodeProcessarAsync(string mensagem)
       {
           return mensagem.Contains("entrega") || mensagem.Contains("rastreio");
       }
   }
   ```

2. Registrar no DI (`Program.cs`):
   ```csharp
   builder.Services.AddScoped<IAgenteEspecialista, AgenteLogistica>();
   ```

3. Criar registro no banco:
   ```sql
   INSERT INTO agente (espaco_id, nome, tipo, palavras_chave_json)
   VALUES ('...', 'Agente Logística', 7, '["entrega", "rastreio", "frete"]');
   ```

### Adicionar Novo Provedor de IA

1. Criar `GrokProvedor.cs`:
   ```csharp
   public class GrokProvedor : IProvedorIA
   {
       public string Nome => "Grok (xAI)";

       public async Task<string> GerarRespostaAsync(string prompt, string? contexto = null)
       {
           // Integração com API do Grok
       }
   }
   ```

2. Atualizar `ProvedorIAFactory.cs`:
   ```csharp
   public static IProvedorIA Criar(ProvedorIA config)
   {
       return config.Tipo switch
       {
           TipoProvedor.OpenAI => new OpenAIProvedor(...),
           TipoProvedor.Anthropic => new ClaudeProvedor(...),
           TipoProvedor.Google => new GeminiProvedor(...),
           TipoProvedor.Grok => new GrokProvedor(...),
           _ => throw new NotSupportedException()
       };
   }
   ```

## 📚 Referências e Documentação

- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Dapper](https://github.com/DapperLib/Dapper)
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
- [OpenAI API](https://platform.openai.com/docs/api-reference)
- [Anthropic Claude API](https://docs.anthropic.com/claude/reference)
- [Google Vertex AI](https://cloud.google.com/vertex-ai/docs)
- [LGPD - Lei Geral de Proteção de Dados](https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)

## 📄 Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 👥 Contribuindo

1. Fork o projeto
2. Crie uma feature branch (`git checkout -b feature/NovaFuncionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/NovaFuncionalidade`)
5. Abra um Pull Request

### Padrões de Código

- **Nomenclatura**: pt-BR (classes, métodos, variáveis)
- **Formatação**: `.editorconfig` (4 espaços, UTF-8, LF)
- **Linting**: Roslyn analyzers habilitados
- **Testes**: Obrigatório para novas funcionalidades (coverage ≥ 80%)

## 🆘 Suporte

- **Issues**: [GitHub Issues](https://github.com/sua-org/evaagent/issues)
- **Discussões**: [GitHub Discussions](https://github.com/sua-org/evaagent/discussions)
- **Email**: suporte@evaagent.local

---

**Desenvolvido com** ❤️ **pela equipe EvaAgent**
