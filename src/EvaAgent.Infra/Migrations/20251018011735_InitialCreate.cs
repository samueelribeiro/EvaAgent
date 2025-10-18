using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvaAgent.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "espaco",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    espaco_pai_id = table.Column<Guid>(type: "uuid", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_espaco", x => x.id);
                    table.ForeignKey(
                        name: "FK_espaco_espaco_espaco_pai_id",
                        column: x => x.espaco_pai_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fila_dead_letter",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fila_mensagem_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_mensagem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    conteudo_json = table.Column<string>(type: "jsonb", nullable: false),
                    erro_mensagem = table.Column<string>(type: "text", nullable: false),
                    stack_trace = table.Column<string>(type: "text", nullable: true),
                    tentativas_processamento = table.Column<int>(type: "integer", nullable: false),
                    enviado_dead_letter_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fila_dead_letter", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "papel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    permissoes = table.Column<string>(type: "jsonb", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_papel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senha_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    avatar = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    time_zone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    idioma = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ultimo_acesso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    email_verificado = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "agente",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    prompt_sistema = table.Column<string>(type: "text", nullable: true),
                    palavras_chave_json = table.Column<string>(type: "jsonb", nullable: true),
                    habilitado = table.Column<bool>(type: "boolean", nullable: false),
                    prioridade = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agente", x => x.id);
                    table.ForeignKey(
                        name: "FK_agente_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "canal",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    configuracao_json = table.Column<string>(type: "jsonb", nullable: true),
                    habilitado = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_canal", x => x.id);
                    table.ForeignKey(
                        name: "FK_canal_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fila_mensagem",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_mensagem = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    conteudo_json = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    tentativas_processamento = table.Column<int>(type: "integer", nullable: false),
                    max_tentativas = table.Column<int>(type: "integer", nullable: false),
                    processado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    erro_mensagem = table.Column<string>(type: "text", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fila_mensagem", x => x.id);
                    table.ForeignKey(
                        name: "FK_fila_mensagem_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grupo_treinamento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupo_treinamento", x => x.id);
                    table.ForeignKey(
                        name: "FK_grupo_treinamento_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "metrica_uso",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_metrica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    recurso = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    quantidade = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unidade_medida = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    dimensoes_json = table.Column<string>(type: "jsonb", nullable: true),
                    medido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metrica_uso", x => x.id);
                    table.ForeignKey(
                        name: "FK_metrica_uso_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "provedor_ia",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    url_base = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    chave_api = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    max_tokens = table.Column<int>(type: "integer", nullable: true),
                    temperatura = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    habilitado = table.Column<bool>(type: "boolean", nullable: false),
                    limite_requisicoes_por_minuto = table.Column<int>(type: "integer", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provedor_ia", x => x.id);
                    table.ForeignKey(
                        name: "FK_provedor_ia_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registro_erro",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: true),
                    codigo_correlacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tipo_erro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    mensagem = table.Column<string>(type: "text", nullable: false),
                    stack_trace = table.Column<string>(type: "text", nullable: true),
                    contexto_json = table.Column<string>(type: "jsonb", nullable: true),
                    severidade = table.Column<int>(type: "integer", nullable: false),
                    ocorrido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolvido = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_erro", x => x.id);
                    table.ForeignKey(
                        name: "FK_registro_erro_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "sistema",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    versao_api = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sistema", x => x.id);
                    table.ForeignKey(
                        name: "FK_sistema_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tarefa_agendada",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo_tarefa = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    parametros_json = table.Column<string>(type: "jsonb", nullable: true),
                    cron_expressao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    proxima_execucao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ultima_execucao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    habilitada = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tarefa_agendada", x => x.id);
                    table.ForeignKey(
                        name: "FK_tarefa_agendada_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registro_auditoria",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    acao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entidade_id = table.Column<Guid>(type: "uuid", nullable: true),
                    valores_antigos = table.Column<string>(type: "jsonb", nullable: true),
                    valores_novos = table.Column<string>(type: "jsonb", nullable: true),
                    ip_origem = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    executado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_auditoria", x => x.id);
                    table.ForeignKey(
                        name: "FK_registro_auditoria_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_registro_auditoria_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "usuario_espaco",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    papel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    convidado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    aceito_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_espaco", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuario_espaco_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuario_espaco_papel_papel_id",
                        column: x => x.papel_id,
                        principalTable: "papel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usuario_espaco_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "intencao_agente",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    agente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    palavras_chave_json = table.Column<string>(type: "jsonb", nullable: false),
                    exemplos_json = table.Column<string>(type: "jsonb", nullable: true),
                    confianca = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_intencao_agente", x => x.id);
                    table.ForeignKey(
                        name: "FK_intencao_agente_agente_agente_id",
                        column: x => x.agente_id,
                        principalTable: "agente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "receptor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    canal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identificador = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    avatar = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    metadados_json = table.Column<string>(type: "jsonb", nullable: true),
                    tom_atendimento = table.Column<string>(type: "text", nullable: false),
                    formato_nome = table.Column<string>(type: "text", nullable: false),
                    usar_saudacao = table.Column<bool>(type: "boolean", nullable: false),
                    idioma = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    time_zone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receptor", x => x.id);
                    table.ForeignKey(
                        name: "FK_receptor_canal_canal_id",
                        column: x => x.canal_id,
                        principalTable: "canal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documento_treinamento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grupo_treinamento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    conteudo_original = table.Column<string>(type: "text", nullable: false),
                    conteudo_processado = table.Column<string>(type: "text", nullable: true),
                    embedding = table.Column<string>(type: "text", nullable: true),
                    metadados_json = table.Column<string>(type: "jsonb", nullable: true),
                    ingerido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documento_treinamento", x => x.id);
                    table.ForeignKey(
                        name: "FK_documento_treinamento_grupo_treinamento_grupo_treinamento_id",
                        column: x => x.grupo_treinamento_id,
                        principalTable: "grupo_treinamento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "acao_negocio",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sistema_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    endpoint_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    metodo_http = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    script_sql = table.Column<string>(type: "text", nullable: true),
                    parametros_json = table.Column<string>(type: "jsonb", nullable: true),
                    requerer_confirmacao = table.Column<bool>(type: "boolean", nullable: false),
                    requerer_autenticacao = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acao_negocio", x => x.id);
                    table.ForeignKey(
                        name: "FK_acao_negocio_sistema_sistema_id",
                        column: x => x.sistema_id,
                        principalTable: "sistema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "conector",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sistema_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    url_base = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    chave_api = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    headers_json = table.Column<string>(type: "jsonb", nullable: true),
                    tipo_banco_dados = table.Column<string>(type: "text", nullable: true),
                    string_conexao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    habilitado = table.Column<bool>(type: "boolean", nullable: false),
                    timeout_segundos = table.Column<int>(type: "integer", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conector", x => x.id);
                    table.ForeignKey(
                        name: "FK_conector_sistema_sistema_id",
                        column: x => x.sistema_id,
                        principalTable: "sistema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "consulta_negocio",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sistema_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    query_sql = table.Column<string>(type: "text", nullable: false),
                    parametros_json = table.Column<string>(type: "jsonb", nullable: true),
                    requerer_autenticacao = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consulta_negocio", x => x.id);
                    table.ForeignKey(
                        name: "FK_consulta_negocio_sistema_sistema_id",
                        column: x => x.sistema_id,
                        principalTable: "sistema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "consentimento_lgpd",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    receptor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    finalidade = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    consentido = table.Column<bool>(type: "boolean", nullable: false),
                    consentido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revogado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ip_origem = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consentimento_lgpd", x => x.id);
                    table.ForeignKey(
                        name: "FK_consentimento_lgpd_receptor_receptor_id",
                        column: x => x.receptor_id,
                        principalTable: "receptor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "conversa",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    receptor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agente_id = table.Column<Guid>(type: "uuid", nullable: true),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    iniciada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finalizada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    resumo_json = table.Column<string>(type: "jsonb", nullable: true),
                    arquivada = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversa", x => x.id);
                    table.ForeignKey(
                        name: "FK_conversa_agente_agente_id",
                        column: x => x.agente_id,
                        principalTable: "agente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_conversa_receptor_receptor_id",
                        column: x => x.receptor_id,
                        principalTable: "receptor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "memoria_longo_prazo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    receptor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chave = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    valor = table.Column<string>(type: "text", nullable: false),
                    categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    importancia_score = table.Column<int>(type: "integer", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memoria_longo_prazo", x => x.id);
                    table.ForeignKey(
                        name: "FK_memoria_longo_prazo_receptor_receptor_id",
                        column: x => x.receptor_id,
                        principalTable: "receptor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "memoria_curto_prazo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chave = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    valor = table.Column<string>(type: "text", nullable: false),
                    expira_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_memoria_curto_prazo", x => x.id);
                    table.ForeignKey(
                        name: "FK_memoria_curto_prazo_conversa_conversa_id",
                        column: x => x.conversa_id,
                        principalTable: "conversa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mensagem",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    direcao = table.Column<string>(type: "text", nullable: false),
                    conteudo = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    enviada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    entregue_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    lida_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    midia_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tipo_midia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    metadados_json = table.Column<string>(type: "jsonb", nullable: true),
                    id_externo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensagem", x => x.id);
                    table.ForeignKey(
                        name: "FK_mensagem_conversa_conversa_id",
                        column: x => x.conversa_id,
                        principalTable: "conversa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registro_execucao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    espaco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversa_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_execucao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    recurso_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nome_recurso = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    parametros_json = table.Column<string>(type: "jsonb", nullable: true),
                    resultado_json = table.Column<string>(type: "jsonb", nullable: true),
                    sucesso = table.Column<bool>(type: "boolean", nullable: false),
                    tempo_execucao_ms = table.Column<int>(type: "integer", nullable: false),
                    executado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_execucao", x => x.id);
                    table.ForeignKey(
                        name: "FK_registro_execucao_conversa_conversa_id",
                        column: x => x.conversa_id,
                        principalTable: "conversa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_registro_execucao_espaco_espaco_id",
                        column: x => x.espaco_id,
                        principalTable: "espaco",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solicitacao_ia",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    provedor_i_a_id = table.Column<Guid>(type: "uuid", nullable: false),
                    conversa_id = table.Column<Guid>(type: "uuid", nullable: true),
                    prompt = table.Column<string>(type: "text", nullable: false),
                    contexto_json = table.Column<string>(type: "jsonb", nullable: true),
                    tokens_prompt = table.Column<int>(type: "integer", nullable: true),
                    solicitado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitacao_ia", x => x.id);
                    table.ForeignKey(
                        name: "FK_solicitacao_ia_conversa_conversa_id",
                        column: x => x.conversa_id,
                        principalTable: "conversa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_solicitacao_ia_provedor_ia_provedor_i_a_id",
                        column: x => x.provedor_i_a_id,
                        principalTable: "provedor_ia",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "registro_pseudonimizacao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    valor_original_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    valor_cifrado = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    tipo_dado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    conversa_id = table.Column<Guid>(type: "uuid", nullable: true),
                    solicitacao_i_a_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pseudonimizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revertido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expira_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_pseudonimizacao", x => x.id);
                    table.ForeignKey(
                        name: "FK_registro_pseudonimizacao_conversa_conversa_id",
                        column: x => x.conversa_id,
                        principalTable: "conversa",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_registro_pseudonimizacao_solicitacao_ia_solicitacao_i_a_id",
                        column: x => x.solicitacao_i_a_id,
                        principalTable: "solicitacao_ia",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "resposta_ia",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    solicitacao_i_a_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resposta = table.Column<string>(type: "text", nullable: false),
                    tokens_resposta = table.Column<int>(type: "integer", nullable: true),
                    custo_estimado = table.Column<decimal>(type: "numeric(10,6)", precision: 10, scale: 6, nullable: true),
                    tempo_resposta_ms = table.Column<int>(type: "integer", nullable: true),
                    respondido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    metadados_json = table.Column<string>(type: "jsonb", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resposta_ia", x => x.id);
                    table.ForeignKey(
                        name: "FK_resposta_ia_solicitacao_ia_solicitacao_i_a_id",
                        column: x => x.solicitacao_i_a_id,
                        principalTable: "solicitacao_ia",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_acao_negocio_ativo",
                table: "acao_negocio",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_acao_negocio_criado_em",
                table: "acao_negocio",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_acao_negocio_nome",
                table: "acao_negocio",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "idx_acao_negocio_sistema",
                table: "acao_negocio",
                column: "sistema_id");

            migrationBuilder.CreateIndex(
                name: "idx_agente_ativo",
                table: "agente",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_agente_criado_em",
                table: "agente",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_agente_espaco",
                table: "agente",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_agente_habilitado",
                table: "agente",
                column: "habilitado");

            migrationBuilder.CreateIndex(
                name: "idx_agente_prioridade",
                table: "agente",
                column: "prioridade");

            migrationBuilder.CreateIndex(
                name: "idx_agente_tipo",
                table: "agente",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "idx_canal_ativo",
                table: "canal",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_canal_criado_em",
                table: "canal",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_canal_espaco",
                table: "canal",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_canal_habilitado",
                table: "canal",
                column: "habilitado");

            migrationBuilder.CreateIndex(
                name: "idx_canal_tipo",
                table: "canal",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "idx_conector_ativo",
                table: "conector",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_conector_criado_em",
                table: "conector",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_conector_habilitado",
                table: "conector",
                column: "habilitado");

            migrationBuilder.CreateIndex(
                name: "idx_conector_sistema",
                table: "conector",
                column: "sistema_id");

            migrationBuilder.CreateIndex(
                name: "idx_conector_tipo",
                table: "conector",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "idx_consentimento_lgpd_ativo",
                table: "consentimento_lgpd",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_consentimento_lgpd_consentido",
                table: "consentimento_lgpd",
                column: "consentido");

            migrationBuilder.CreateIndex(
                name: "idx_consentimento_lgpd_criado_em",
                table: "consentimento_lgpd",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_consentimento_lgpd_data",
                table: "consentimento_lgpd",
                column: "consentido_em");

            migrationBuilder.CreateIndex(
                name: "idx_consentimento_lgpd_receptor",
                table: "consentimento_lgpd",
                column: "receptor_id");

            migrationBuilder.CreateIndex(
                name: "idx_consentimento_lgpd_receptor_finalidade",
                table: "consentimento_lgpd",
                columns: new[] { "receptor_id", "finalidade" });

            migrationBuilder.CreateIndex(
                name: "idx_consulta_negocio_ativo",
                table: "consulta_negocio",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_consulta_negocio_criado_em",
                table: "consulta_negocio",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_consulta_negocio_nome",
                table: "consulta_negocio",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "idx_consulta_negocio_sistema",
                table: "consulta_negocio",
                column: "sistema_id");

            migrationBuilder.CreateIndex(
                name: "idx_conversa_agente",
                table: "conversa",
                column: "agente_id");

            migrationBuilder.CreateIndex(
                name: "idx_conversa_arquivada",
                table: "conversa",
                column: "arquivada");

            migrationBuilder.CreateIndex(
                name: "idx_conversa_ativo",
                table: "conversa",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_conversa_criado_em",
                table: "conversa",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_conversa_iniciada",
                table: "conversa",
                column: "iniciada_em");

            migrationBuilder.CreateIndex(
                name: "idx_conversa_receptor",
                table: "conversa",
                column: "receptor_id");

            migrationBuilder.CreateIndex(
                name: "idx_documento_treinamento_ativo",
                table: "documento_treinamento",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_documento_treinamento_criado_em",
                table: "documento_treinamento",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_documento_treinamento_grupo",
                table: "documento_treinamento",
                column: "grupo_treinamento_id");

            migrationBuilder.CreateIndex(
                name: "idx_documento_treinamento_ingerido",
                table: "documento_treinamento",
                column: "ingerido_em");

            migrationBuilder.CreateIndex(
                name: "idx_documento_treinamento_nome",
                table: "documento_treinamento",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "idx_espaco_ativo",
                table: "espaco",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_espaco_criado_em",
                table: "espaco",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_espaco_pai",
                table: "espaco",
                column: "espaco_pai_id");

            migrationBuilder.CreateIndex(
                name: "idx_espaco_slug",
                table: "espaco",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_fila_dead_letter_ativo",
                table: "fila_dead_letter",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_fila_dead_letter_criado_em",
                table: "fila_dead_letter",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_fila_dead_letter_enviado",
                table: "fila_dead_letter",
                column: "enviado_dead_letter_em");

            migrationBuilder.CreateIndex(
                name: "idx_fila_dead_letter_mensagem",
                table: "fila_dead_letter",
                column: "fila_mensagem_id");

            migrationBuilder.CreateIndex(
                name: "idx_fila_dead_letter_tipo",
                table: "fila_dead_letter",
                column: "tipo_mensagem");

            migrationBuilder.CreateIndex(
                name: "idx_fila_mensagem_ativo",
                table: "fila_mensagem",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_fila_mensagem_criado_em",
                table: "fila_mensagem",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_fila_mensagem_espaco",
                table: "fila_mensagem",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_fila_mensagem_processado",
                table: "fila_mensagem",
                column: "processado_em");

            migrationBuilder.CreateIndex(
                name: "idx_fila_mensagem_status",
                table: "fila_mensagem",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_fila_mensagem_tipo",
                table: "fila_mensagem",
                column: "tipo_mensagem");

            migrationBuilder.CreateIndex(
                name: "idx_grupo_treinamento_ativo",
                table: "grupo_treinamento",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_grupo_treinamento_criado_em",
                table: "grupo_treinamento",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_grupo_treinamento_espaco",
                table: "grupo_treinamento",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_grupo_treinamento_nome",
                table: "grupo_treinamento",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "idx_intencao_agente",
                table: "intencao_agente",
                column: "agente_id");

            migrationBuilder.CreateIndex(
                name: "idx_intencao_agente_ativo",
                table: "intencao_agente",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_intencao_agente_criado_em",
                table: "intencao_agente",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_intencao_confianca",
                table: "intencao_agente",
                column: "confianca");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_curto_prazo_ativo",
                table: "memoria_curto_prazo",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_curto_prazo_conversa",
                table: "memoria_curto_prazo",
                column: "conversa_id");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_curto_prazo_conversa_chave",
                table: "memoria_curto_prazo",
                columns: new[] { "conversa_id", "chave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_memoria_curto_prazo_criado_em",
                table: "memoria_curto_prazo",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_curto_prazo_expira",
                table: "memoria_curto_prazo",
                column: "expira_em");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_longo_prazo_ativo",
                table: "memoria_longo_prazo",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_longo_prazo_categoria",
                table: "memoria_longo_prazo",
                column: "categoria");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_longo_prazo_criado_em",
                table: "memoria_longo_prazo",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_longo_prazo_importancia",
                table: "memoria_longo_prazo",
                column: "importancia_score");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_longo_prazo_receptor",
                table: "memoria_longo_prazo",
                column: "receptor_id");

            migrationBuilder.CreateIndex(
                name: "idx_memoria_longo_prazo_receptor_chave",
                table: "memoria_longo_prazo",
                columns: new[] { "receptor_id", "chave" });

            migrationBuilder.CreateIndex(
                name: "idx_mensagem_ativo",
                table: "mensagem",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_mensagem_conversa",
                table: "mensagem",
                column: "conversa_id");

            migrationBuilder.CreateIndex(
                name: "idx_mensagem_criado_em",
                table: "mensagem",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_mensagem_enviada",
                table: "mensagem",
                column: "enviada_em");

            migrationBuilder.CreateIndex(
                name: "idx_mensagem_externo",
                table: "mensagem",
                column: "id_externo");

            migrationBuilder.CreateIndex(
                name: "idx_mensagem_status",
                table: "mensagem",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_metrica_uso_ativo",
                table: "metrica_uso",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_metrica_uso_criado_em",
                table: "metrica_uso",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_metrica_uso_espaco",
                table: "metrica_uso",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_metrica_uso_espaco_tipo_data",
                table: "metrica_uso",
                columns: new[] { "espaco_id", "tipo_metrica", "medido_em" });

            migrationBuilder.CreateIndex(
                name: "idx_metrica_uso_medido",
                table: "metrica_uso",
                column: "medido_em");

            migrationBuilder.CreateIndex(
                name: "idx_metrica_uso_recurso",
                table: "metrica_uso",
                column: "recurso");

            migrationBuilder.CreateIndex(
                name: "idx_metrica_uso_tipo",
                table: "metrica_uso",
                column: "tipo_metrica");

            migrationBuilder.CreateIndex(
                name: "idx_papel_ativo",
                table: "papel",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_papel_criado_em",
                table: "papel",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_papel_nome",
                table: "papel",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "idx_papel_tipo",
                table: "papel",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "idx_provedor_ia_ativo",
                table: "provedor_ia",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_provedor_ia_criado_em",
                table: "provedor_ia",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_provedor_ia_espaco",
                table: "provedor_ia",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_provedor_ia_habilitado",
                table: "provedor_ia",
                column: "habilitado");

            migrationBuilder.CreateIndex(
                name: "idx_provedor_ia_tipo",
                table: "provedor_ia",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "idx_receptor_ativo",
                table: "receptor",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_receptor_canal",
                table: "receptor",
                column: "canal_id");

            migrationBuilder.CreateIndex(
                name: "idx_receptor_canal_identificador",
                table: "receptor",
                columns: new[] { "canal_id", "identificador" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_receptor_criado_em",
                table: "receptor",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_receptor_email",
                table: "receptor",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "idx_registro_auditoria_acao",
                table: "registro_auditoria",
                column: "acao");

            migrationBuilder.CreateIndex(
                name: "idx_registro_auditoria_ativo",
                table: "registro_auditoria",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_registro_auditoria_criado_em",
                table: "registro_auditoria",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_registro_auditoria_entidade",
                table: "registro_auditoria",
                column: "entidade");

            migrationBuilder.CreateIndex(
                name: "idx_registro_auditoria_entidade_id",
                table: "registro_auditoria",
                column: "entidade_id");

            migrationBuilder.CreateIndex(
                name: "idx_registro_auditoria_espaco",
                table: "registro_auditoria",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_registro_auditoria_executado",
                table: "registro_auditoria",
                column: "executado_em");

            migrationBuilder.CreateIndex(
                name: "idx_registro_auditoria_usuario",
                table: "registro_auditoria",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "idx_registro_erro_ativo",
                table: "registro_erro",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_registro_erro_correlacao",
                table: "registro_erro",
                column: "codigo_correlacao");

            migrationBuilder.CreateIndex(
                name: "idx_registro_erro_criado_em",
                table: "registro_erro",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_registro_erro_espaco",
                table: "registro_erro",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_registro_erro_ocorrido",
                table: "registro_erro",
                column: "ocorrido_em");

            migrationBuilder.CreateIndex(
                name: "idx_registro_erro_resolvido",
                table: "registro_erro",
                column: "resolvido");

            migrationBuilder.CreateIndex(
                name: "idx_registro_erro_severidade",
                table: "registro_erro",
                column: "severidade");

            migrationBuilder.CreateIndex(
                name: "idx_registro_erro_tipo",
                table: "registro_erro",
                column: "tipo_erro");

            migrationBuilder.CreateIndex(
                name: "idx_registro_execucao_ativo",
                table: "registro_execucao",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_registro_execucao_conversa",
                table: "registro_execucao",
                column: "conversa_id");

            migrationBuilder.CreateIndex(
                name: "idx_registro_execucao_criado_em",
                table: "registro_execucao",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_registro_execucao_espaco",
                table: "registro_execucao",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_registro_execucao_espaco_tipo_data",
                table: "registro_execucao",
                columns: new[] { "espaco_id", "tipo_execucao", "executado_em" });

            migrationBuilder.CreateIndex(
                name: "idx_registro_execucao_executado",
                table: "registro_execucao",
                column: "executado_em");

            migrationBuilder.CreateIndex(
                name: "idx_registro_execucao_recurso",
                table: "registro_execucao",
                column: "recurso_id");

            migrationBuilder.CreateIndex(
                name: "idx_registro_execucao_sucesso",
                table: "registro_execucao",
                column: "sucesso");

            migrationBuilder.CreateIndex(
                name: "idx_registro_execucao_tipo",
                table: "registro_execucao",
                column: "tipo_execucao");

            migrationBuilder.CreateIndex(
                name: "idx_registro_pseudonimizacao_ativo",
                table: "registro_pseudonimizacao",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_registro_pseudonimizacao_conversa",
                table: "registro_pseudonimizacao",
                column: "conversa_id");

            migrationBuilder.CreateIndex(
                name: "idx_registro_pseudonimizacao_criado_em",
                table: "registro_pseudonimizacao",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_registro_pseudonimizacao_expira",
                table: "registro_pseudonimizacao",
                column: "expira_em");

            migrationBuilder.CreateIndex(
                name: "idx_registro_pseudonimizacao_guid",
                table: "registro_pseudonimizacao",
                column: "guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_registro_pseudonimizacao_hash",
                table: "registro_pseudonimizacao",
                column: "valor_original_hash");

            migrationBuilder.CreateIndex(
                name: "idx_registro_pseudonimizacao_solicitacao",
                table: "registro_pseudonimizacao",
                column: "solicitacao_i_a_id");

            migrationBuilder.CreateIndex(
                name: "idx_resposta_ia_ativo",
                table: "resposta_ia",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_resposta_ia_criado_em",
                table: "resposta_ia",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_resposta_ia_data",
                table: "resposta_ia",
                column: "respondido_em");

            migrationBuilder.CreateIndex(
                name: "idx_resposta_ia_solicitacao",
                table: "resposta_ia",
                column: "solicitacao_i_a_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_sistema_ativo",
                table: "sistema",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_sistema_criado_em",
                table: "sistema",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_sistema_espaco",
                table: "sistema",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_sistema_nome",
                table: "sistema",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "idx_solicitacao_ia_ativo",
                table: "solicitacao_ia",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_solicitacao_ia_conversa",
                table: "solicitacao_ia",
                column: "conversa_id");

            migrationBuilder.CreateIndex(
                name: "idx_solicitacao_ia_criado_em",
                table: "solicitacao_ia",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_solicitacao_ia_data",
                table: "solicitacao_ia",
                column: "solicitado_em");

            migrationBuilder.CreateIndex(
                name: "idx_solicitacao_ia_provedor",
                table: "solicitacao_ia",
                column: "provedor_i_a_id");

            migrationBuilder.CreateIndex(
                name: "idx_tarefa_agendada_ativo",
                table: "tarefa_agendada",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_tarefa_agendada_criado_em",
                table: "tarefa_agendada",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_tarefa_agendada_espaco",
                table: "tarefa_agendada",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_tarefa_agendada_habilitada",
                table: "tarefa_agendada",
                column: "habilitada");

            migrationBuilder.CreateIndex(
                name: "idx_tarefa_agendada_proxima",
                table: "tarefa_agendada",
                column: "proxima_execucao");

            migrationBuilder.CreateIndex(
                name: "idx_tarefa_agendada_status",
                table: "tarefa_agendada",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_ativo",
                table: "usuario",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_criado_em",
                table: "usuario",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_email",
                table: "usuario",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_usuario_espaco_ativo",
                table: "usuario_espaco",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_espaco_criado_em",
                table: "usuario_espaco",
                column: "criado_em");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_espaco_espaco",
                table: "usuario_espaco",
                column: "espaco_id");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_espaco_papel",
                table: "usuario_espaco",
                column: "papel_id");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_espaco_unique",
                table: "usuario_espaco",
                columns: new[] { "usuario_id", "espaco_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acao_negocio");

            migrationBuilder.DropTable(
                name: "conector");

            migrationBuilder.DropTable(
                name: "consentimento_lgpd");

            migrationBuilder.DropTable(
                name: "consulta_negocio");

            migrationBuilder.DropTable(
                name: "documento_treinamento");

            migrationBuilder.DropTable(
                name: "fila_dead_letter");

            migrationBuilder.DropTable(
                name: "fila_mensagem");

            migrationBuilder.DropTable(
                name: "intencao_agente");

            migrationBuilder.DropTable(
                name: "memoria_curto_prazo");

            migrationBuilder.DropTable(
                name: "memoria_longo_prazo");

            migrationBuilder.DropTable(
                name: "mensagem");

            migrationBuilder.DropTable(
                name: "metrica_uso");

            migrationBuilder.DropTable(
                name: "registro_auditoria");

            migrationBuilder.DropTable(
                name: "registro_erro");

            migrationBuilder.DropTable(
                name: "registro_execucao");

            migrationBuilder.DropTable(
                name: "registro_pseudonimizacao");

            migrationBuilder.DropTable(
                name: "resposta_ia");

            migrationBuilder.DropTable(
                name: "tarefa_agendada");

            migrationBuilder.DropTable(
                name: "usuario_espaco");

            migrationBuilder.DropTable(
                name: "sistema");

            migrationBuilder.DropTable(
                name: "grupo_treinamento");

            migrationBuilder.DropTable(
                name: "solicitacao_ia");

            migrationBuilder.DropTable(
                name: "papel");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "conversa");

            migrationBuilder.DropTable(
                name: "provedor_ia");

            migrationBuilder.DropTable(
                name: "agente");

            migrationBuilder.DropTable(
                name: "receptor");

            migrationBuilder.DropTable(
                name: "canal");

            migrationBuilder.DropTable(
                name: "espaco");
        }
    }
}
