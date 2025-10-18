namespace EvaAgent.Dominio.Interfaces.Servicos;

public interface IProvedorIA
{
    string Nome { get; }
    Task<string> GerarRespostaAsync(string prompt, string? contexto = null, CancellationToken cancellationToken = default);
    Task<(string Resposta, int TokensPrompt, int TokensResposta, decimal CustoEstimado)> GerarRespostaDetalhadaAsync(
        string prompt,
        string? contexto = null,
        CancellationToken cancellationToken = default);
}
