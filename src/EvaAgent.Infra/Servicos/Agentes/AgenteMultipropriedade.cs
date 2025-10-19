using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.Agentes;

/// <summary>
/// Agente especialista em multipropriedade e timeshare
/// </summary>
public class AgenteMultipropriedade : IAgenteEspecialista
{
    private readonly ILogger<AgenteMultipropriedade> _logger;

    public string Nome => "Agente de Multipropriedade";

    public AgenteMultipropriedade(ILogger<AgenteMultipropriedade> logger)
    {
        _logger = logger;
    }

    public async Task<string> ProcessarMensagemAsync(
        string mensagem,
        Guid conversaId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processando mensagem com Agente de Multipropriedade - Conversa: {ConversaId}", conversaId);

        // TODO: Integrar com provedor IA e contexto
        await Task.CompletedTask;
        return $"[Agente de Multipropriedade] Entendi sua solicitação sobre: {mensagem}";
    }

    public Task<bool> PodeProcessarAsync(string mensagem, CancellationToken cancellationToken = default)
    {
        var palavrasChave = new[]
        {
            "multipropriedade", "timeshare", "semana", "intercâmbio",
            "RCI", "cota", "fração", "condomínio", "manutenção",
            "anuidade", "permuta", "high season", "baixa temporada"
        };

        var mensagemLower = mensagem.ToLowerInvariant();
        var pode = palavrasChave.Any(p => mensagemLower.Contains(p));

        return Task.FromResult(pode);
    }
}
