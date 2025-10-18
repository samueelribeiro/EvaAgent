using System.Text.Json;
using EvaAgent.Dominio.Entidades.Agentes;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.Agentes;

/// <summary>
/// Serviço para resolver intenções e rotear para agentes especialistas
/// </summary>
public class IntentResolverService : IIntentResolverService
{
    private readonly IRepositorioBase<Agente> _repositorio;
    private readonly ILogger<IntentResolverService> _logger;

    public IntentResolverService(
        IRepositorioBase<Agente> repositorio,
        ILogger<IntentResolverService> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<(Agente? Agente, decimal Confianca)> ResolverIntencaoAsync(
        string mensagem,
        Guid espacoId,
        CancellationToken cancellationToken = default)
    {
        var resultados = await ResolverIntencoesAsync(mensagem, espacoId, 1, cancellationToken);
        var melhor = resultados.FirstOrDefault();

        return melhor != default ? (melhor.Agente, melhor.Confianca) : (null, 0);
    }

    public async Task<List<(Agente Agente, decimal Confianca)>> ResolverIntencoesAsync(
        string mensagem,
        Guid espacoId,
        int top = 3,
        CancellationToken cancellationToken = default)
    {
        // Buscar agentes habilitados do espaço
        var agentes = await _repositorio.BuscarAsync(
            a => a.EspacoId == espacoId && a.Habilitado,
            cancellationToken);

        if (!agentes.Any())
        {
            _logger.LogWarning("Nenhum agente habilitado encontrado para o espaço {EspacoId}", espacoId);
            return new List<(Agente, decimal)>();
        }

        // Tokenizar mensagem
        var tokens = Tokenizar(mensagem);

        // Calcular score para cada agente
        var scores = new List<(Agente Agente, decimal Score)>();

        foreach (var agente in agentes)
        {
            var palavrasChave = DeserializarPalavrasChave(agente.PalavrasChaveJson);
            var score = CalcularScore(tokens, palavrasChave, agente.Prioridade);

            scores.Add((agente, score));

            _logger.LogDebug(
                "Agente {Agente}: Score {Score:F2}",
                agente.Nome,
                score);
        }

        // Ordenar por score e retornar top N
        var resultados = scores
            .OrderByDescending(s => s.Score)
            .Take(top)
            .Where(s => s.Score >= 0.3m) // Threshold mínimo
            .Select(s => (s.Agente, s.Score))
            .ToList();

        if (resultados.Any())
        {
            _logger.LogInformation(
                "Intenção resolvida para agente {Agente} com confiança {Confianca:F2}",
                resultados[0].Agente.Nome,
                resultados[0].Score);
        }
        else
        {
            _logger.LogWarning("Nenhum agente com score suficiente para: {Mensagem}", mensagem);
        }

        return resultados;
    }

    private static List<string> Tokenizar(string mensagem)
    {
        return mensagem
            .ToLowerInvariant()
            .Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '\n', '\r' },
                   StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length > 2) // Ignorar palavras muito curtas
            .ToList();
    }

    private static string[] DeserializarPalavrasChave(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return Array.Empty<string>();

        try
        {
            return JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    private static decimal CalcularScore(
        List<string> tokens,
        string[] palavrasChave,
        int prioridade)
    {
        if (palavrasChave.Length == 0)
            return 0m;

        // Agente geral (curinga)
        if (palavrasChave.Contains("*"))
            return 0.4m + (prioridade * 0.01m); // Score baixo, só fallback

        // Contar matches
        var matches = tokens.Count(t =>
            palavrasChave.Any(pc => pc.ToLowerInvariant().Contains(t) || t.Contains(pc.ToLowerInvariant()))
        );

        if (matches == 0)
            return 0m;

        // Score baseado em: % de matches + prioridade
        var percentualMatch = (decimal)matches / tokens.Count;
        var scorePrioridade = prioridade * 0.02m; // Cada ponto de prioridade adiciona 2%

        return Math.Min(1.0m, percentualMatch * 0.9m + scorePrioridade);
    }

    public async Task<Agente?> ResolverAgenteAsync(string mensagem, Guid espacoId)
    {
        var (agente, _) = await ResolverIntencaoAsync(mensagem, espacoId);
        return agente;
    }
}
