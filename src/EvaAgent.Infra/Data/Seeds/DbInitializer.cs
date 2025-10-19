using EvaAgent.Dominio.Entidades.Agentes;
using EvaAgent.Dominio.Entidades.Canais;
using EvaAgent.Dominio.Entidades.IA;
using EvaAgent.Dominio.Entidades.Identidade;
using EvaAgent.Dominio.Enums;
using EvaAgent.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EvaAgent.Infra.Data.Seeds;

/// <summary>
/// Inicializador do banco de dados com dados seed
/// </summary>
public static class DbInitializer
{
    public static async Task InicializarAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            logger.LogInformation("Iniciando seed do banco de dados...");

            // Aplicar migrations pendentes
            if (context.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("Aplicando migrations pendentes...");
                await context.Database.MigrateAsync();
            }

            // Verificar se já existe dados
            if (await context.Espacos.AnyAsync())
            {
                logger.LogInformation("Banco de dados já possui dados. Seed ignorado.");
                return;
            }

            await SeedPapeisAsync(context, logger);
            await SeedEspacosAsync(context, logger);
            await SeedUsuariosAsync(context, logger);
            await SeedCanaisAsync(context, logger);
            await SeedProvedoresIAAsync(context, logger);
            await SeedAgentesAsync(context, logger);

            await context.SaveChangesAsync();

            logger.LogInformation("Seed do banco de dados concluído com sucesso!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar seed do banco de dados");
            throw;
        }
    }

    private static async Task SeedPapeisAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Criando papéis...");

        var papeis = new[]
        {
            new Papel
            {
                Id = Guid.NewGuid(),
                Nome = "Administrador",
                Descricao = "Acesso total ao sistema",
                Tipo = TipoPapel.Administrador
            },
            new Papel
            {
                Id = Guid.NewGuid(),
                Nome = "Membro",
                Descricao = "Membro do espaço",
                Tipo = TipoPapel.Membro
            }
        };

        await context.Papeis.AddRangeAsync(papeis);
        logger.LogInformation("Criados {Quantidade} papéis", papeis.Length);
    }

    private static async Task SeedEspacosAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Criando espaços...");

        var espacoPrincipal = new Espaco
        {
            Id = Guid.NewGuid(),
            Nome = "Organização Principal",
            Descricao = "Espaço raiz da organização",
            EspacoPaiId = null
        };

        await context.Espacos.AddAsync(espacoPrincipal);

        var espacoFilho = new Espaco
        {
            Id = Guid.NewGuid(),
            Nome = "Departamento de Atendimento",
            Descricao = "Espaço do departamento de atendimento",
            EspacoPaiId = espacoPrincipal.Id
        };

        await context.Espacos.AddAsync(espacoFilho);

        logger.LogInformation("Criados 2 espaços");
    }

    private static async Task SeedUsuariosAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Criando usuários...");

        var espaco = await context.Espacos.FirstAsync();
        var papelAdmin = await context.Papeis.FirstAsync(p => p.Tipo == TipoPapel.Administrador);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Administrador",
            Email = "admin@evaagent.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // Senha padrão
            EmailVerificado = true,
            Idioma = "pt-BR"
        };

        await context.Usuarios.AddAsync(usuario);

        var usuarioEspaco = new UsuarioEspaco
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            EspacoId = espaco.Id,
            PapelId = papelAdmin.Id
        };

        await context.UsuarioEspacos.AddAsync(usuarioEspaco);

        logger.LogInformation("Criado usuário administrador (admin@evaagent.com / Admin@123)");
    }

    private static async Task SeedCanaisAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Criando canais...");

        var espaco = await context.Espacos.FirstAsync();

        var canais = new[]
        {
            new Canal
            {
                Id = Guid.NewGuid(),
                EspacoId = espaco.Id,
                Nome = "WhatsApp Business",
                Tipo = TipoCanal.WhatsApp,
                ConfiguracaoJson = JsonSerializer.Serialize(new
                {
                    phoneNumberId = "",
                    accessToken = "",
                    webhookToken = ""
                }),
                Habilitado = true
            },
            new Canal
            {
                Id = Guid.NewGuid(),
                EspacoId = espaco.Id,
                Nome = "WebChat",
                Tipo = TipoCanal.WebChat,
                ConfiguracaoJson = JsonSerializer.Serialize(new
                {
                    widgetId = Guid.NewGuid().ToString()
                }),
                Habilitado = true
            }
        };

        await context.Canais.AddRangeAsync(canais);
        logger.LogInformation("Criados {Quantidade} canais", canais.Length);
    }

    private static async Task SeedProvedoresIAAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Criando provedores de IA...");

        var espaco = await context.Espacos.FirstAsync();

        var provedores = new[]
        {
            new ProvedorIA
            {
                Id = Guid.NewGuid(),
                EspacoId = espaco.Id,
                Nome = "OpenAI GPT-4",
                Tipo = TipoProvedor.OpenAI,
                Modelo = "gpt-4",
                ChaveApi = "", // Deve ser configurado pelo usuário
                MaxTokens = 2000,
                Temperatura = 0.7m,
                Habilitado = true
            },
            new ProvedorIA
            {
                Id = Guid.NewGuid(),
                EspacoId = espaco.Id,
                Nome = "Claude 3 Sonnet",
                Tipo = TipoProvedor.Anthropic,
                Modelo = "claude-3-sonnet-20240229",
                ChaveApi = "",
                MaxTokens = 2000,
                Temperatura = 0.7m,
                Habilitado = false
            },
            new ProvedorIA
            {
                Id = Guid.NewGuid(),
                EspacoId = espaco.Id,
                Nome = "Google Gemini Pro",
                Tipo = TipoProvedor.Google,
                Modelo = "gemini-pro",
                ChaveApi = "",
                MaxTokens = 2000,
                Temperatura = 0.7m,
                Habilitado = false
            }
        };

        await context.ProvedoresIA.AddRangeAsync(provedores);
        logger.LogInformation("Criados {Quantidade} provedores de IA", provedores.Length);
    }

    private static async Task SeedAgentesAsync(AppDbContext context, ILogger logger)
    {
        logger.LogInformation("Criando agentes especializados...");

        var espaco = await context.Espacos.FirstAsync();
        var provedor = await context.ProvedoresIA.FirstAsync();

        var agentes = new[]
        {
            new Agente
            {
                Id = Guid.NewGuid(),
                EspacoId = espaco.Id,
Nome = "Agente Financeiro",
                Descricao = "Especialista em consultas financeiras e relatórios",
                Tipo = TipoAgente.Financeiro,
                PalavrasChaveJson = JsonSerializer.Serialize(new[]
                {
                    "vendas", "faturamento", "receita", "despesa", "lucro",
                    "balanço", "fluxo de caixa", "relatório financeiro", "finanças"
                }),
                PromptSistema = "Você é um assistente financeiro especializado.",
Prioridade = 10,
                Habilitado = true
            },
            new Agente
            {
                Id = Guid.NewGuid(),
                EspacoId = espaco.Id,
Nome = "Agente de Hotelaria",
                Descricao = "Especialista em atendimento hoteleiro",
                Tipo = TipoAgente.Geral,
                PalavrasChaveJson = JsonSerializer.Serialize(new[]
                {
                    "reserva", "quarto", "hotel", "check-in", "check-out",
                    "hospedagem", "diária", "suite"
                }),
                PromptSistema = "Você é um assistente especializado em hotelaria.",
Prioridade = 8,
                Habilitado = true
            },
            new Agente
            {
                Id = Guid.NewGuid(),
                EspacoId = espaco.Id,
Nome = "Agente de Suporte",
                Descricao = "Especialista em suporte técnico",
                Tipo = TipoAgente.Geral,
                PalavrasChaveJson = JsonSerializer.Serialize(new[]
                {
                    "problema", "erro", "não funciona", "ajuda", "suporte",
                    "bug", "como faço"
                }),
                PromptSistema = "Você é um agente de suporte técnico especializado.",
Prioridade = 7,
                Habilitado = true
            },
            new Agente
            {
                Id = Guid.NewGuid(),
                EspacoId = espaco.Id,
Nome = "Agente Geral",
                Descricao = "Agente para atendimento geral (fallback)",
                Tipo = TipoAgente.Geral,
                PalavrasChaveJson = JsonSerializer.Serialize(new[] { "*" }), // Curinga
                PromptSistema = "Você é um assistente virtual prestativo e cortês.",
Prioridade = 1,
                Habilitado = true
            }
        };

        await context.Agentes.AddRangeAsync(agentes);
        logger.LogInformation("Criados {Quantidade} agentes", agentes.Length);
    }
}
