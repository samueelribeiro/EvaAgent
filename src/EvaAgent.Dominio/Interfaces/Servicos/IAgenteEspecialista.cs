namespace EvaAgent.Dominio.Interfaces.Servicos;

public interface IAgenteEspecialista
{
    string Nome { get; }
    Task<string> ProcessarMensagemAsync(string mensagem, Guid conversaId, CancellationToken cancellationToken = default);
    Task<bool> PodeProcessarAsync(string mensagem, CancellationToken cancellationToken = default);
}
