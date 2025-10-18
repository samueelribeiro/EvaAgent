namespace EvaAgent.Dominio.Interfaces.Servicos;

public interface IDocumentoIngestor
{
    Task<Guid> IngerirDocumentoAsync(Guid grupoTreinamentoId, string nome, string conteudo, string? descricao = null);
    Task<List<string>> BuscarSemanticaAsync(Guid grupoTreinamentoId, string consulta, int top = 5);
    Task<bool> RemoverDocumentoAsync(Guid documentoId);
}
