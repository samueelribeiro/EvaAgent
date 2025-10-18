namespace EvaAgent.Dominio.Interfaces.Servicos;

public interface IMemoriaService
{
    // Memória de curto prazo (escopo de conversa)
    Task ArmazenarCurtoPrazoAsync(Guid conversaId, string chave, string valor, TimeSpan? expiracao = null);
    Task<string?> ObterCurtoPrazoAsync(Guid conversaId, string chave);
    Task RemoverCurtoPrazoAsync(Guid conversaId, string chave);

    // Memória de longo prazo (escopo de receptor)
    Task ArmazenarLongoPrazoAsync(Guid receptorId, string chave, string valor, string? categoria = null, int importancia = 0);
    Task<string?> ObterLongoPrazoAsync(Guid receptorId, string chave);
    Task<Dictionary<string, string>> ObterTodasLongoPrazoAsync(Guid receptorId, string? categoria = null);
    Task RemoverLongoPrazoAsync(Guid receptorId, string chave);
}
