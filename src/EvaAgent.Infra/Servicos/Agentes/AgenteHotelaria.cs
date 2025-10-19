using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.Agentes;

/// <summary>
/// Agente especialista em atendimento hoteleiro
/// </summary>
public class AgenteHotelaria : IAgenteEspecialista
{
    private readonly ILogger<AgenteHotelaria> _logger;

    public string Nome => "Agente de Hotelaria";

    public AgenteHotelaria(ILogger<AgenteHotelaria> logger)
    {
        _logger = logger;
    }

    public async Task<string> ProcessarMensagemAsync(
        string mensagem,
        Guid conversaId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processando mensagem com Agente de Hotelaria - Conversa: {ConversaId}", conversaId);

        // TODO: Integrar com provedor IA e contexto
        await Task.CompletedTask;
        return $"[Agente de Hotelaria] Entendi sua solicitação sobre: {mensagem}";
    }

    public Task<bool> PodeProcessarAsync(string mensagem, CancellationToken cancellationToken = default)
    {
        var palavrasChave = new[]
        {
            "reserva", "quarto", "hotel", "check-in", "check-out",
            "hospedagem", "diária", "suite", "amenidade", "café da manhã",
            "frigobar", "serviço de quarto", "concierge", "recepção"
        };

        var mensagemLower = mensagem.ToLowerInvariant();
        var pode = palavrasChave.Any(p => mensagemLower.Contains(p));

        return Task.FromResult(pode);
    }
}
