using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.Agentes;

/// <summary>
/// Agente geral para atendimento genérico (fallback)
/// </summary>
public class AgenteGeral : IAgenteEspecialista
{
    private readonly ILogger<AgenteGeral> _logger;
    private readonly IProvedorIA _provedorIA;

    public string Nome => "Agente Geral";

    public AgenteGeral(ILogger<AgenteGeral> logger, IProvedorIA provedorIA)
    {
        _logger = logger;
        _provedorIA = provedorIA;
    }

    public async Task<string> ProcessarMensagemAsync(
        string mensagem,
        Guid conversaId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processando mensagem com Agente Geral");

        var promptSistema = @"Você é um assistente virtual prestativo e cortês.
Você auxilia com perguntas gerais e direciona para especialistas quando necessário.
Quando não souber uma resposta específica, seja honesto e sugira alternativas.
Mantenha um tom profissional e amigável.";

        // Por enquanto, retorna uma resposta simples
        // TODO: Integrar com sistema de contexto e provedor IA configurado
        return $"Recebi sua mensagem: {mensagem}. Como posso ajudá-lo?";
    }

    public Task<bool> PodeProcessarAsync(string mensagem, CancellationToken cancellationToken = default)
    {
        // Agente geral sempre pode processar (fallback)
        return Task.FromResult(true);
    }
}
