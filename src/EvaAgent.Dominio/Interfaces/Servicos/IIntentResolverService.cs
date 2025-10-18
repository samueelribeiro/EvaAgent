using EvaAgent.Dominio.Entidades.Agentes;

namespace EvaAgent.Dominio.Interfaces.Servicos;

public interface IIntentResolverService
{
    Task<(Agente? Agente, decimal Confianca)> ResolverIntencaoAsync(string mensagem, Guid espacoId, CancellationToken cancellationToken = default);
    Task<List<(Agente Agente, decimal Confianca)>> ResolverIntencoesAsync(string mensagem, Guid espacoId, int top = 3, CancellationToken cancellationToken = default);
    Task<Agente?> ResolverAgenteAsync(string mensagem, Guid espacoId);
}
