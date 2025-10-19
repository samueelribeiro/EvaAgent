using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.Agentes;

/// <summary>
/// Agente especialista em suporte técnico e atendimento ao cliente
/// </summary>
public class AgenteSuporte : IAgenteEspecialista
{
    private readonly ILogger<AgenteSuporte> _logger;

    public string Nome => "Agente de Suporte";

    public AgenteSuporte(ILogger<AgenteSuporte> logger)
    {
        _logger = logger;
    }

    public async Task<string> ProcessarMensagemAsync(
        string mensagem,
        Guid conversaId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processando mensagem com Agente de Suporte - Conversa: {ConversaId}", conversaId);

        // TODO: Integrar com provedor IA e contexto
        await Task.CompletedTask;
        return $"[Agente de Suporte] Entendi seu problema: {mensagem}. Vou ajudá-lo a resolver.";
    }

    public Task<bool> PodeProcessarAsync(string mensagem, CancellationToken cancellationToken = default)
    {
        var palavrasChave = new[]
        {
            "problema", "erro", "não funciona", "bug", "ajuda",
            "suporte", "não consigo", "como faço", "tutorial",
            "passo a passo", "não está funcionando", "travou",
            "lento", "atualização", "versão", "configuração"
        };

        var mensagemLower = mensagem.ToLowerInvariant();
        var pode = palavrasChave.Any(p => mensagemLower.Contains(p));

        return Task.FromResult(pode);
    }
}
