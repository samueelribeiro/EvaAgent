namespace EvaAgent.Dominio.Interfaces.Servicos;

public interface IFilaService
{
    Task EnfileirarAsync(Guid espacoId, string tipoMensagem, object conteudo);
    Task<(Guid Id, string TipoMensagem, string ConteudoJson)?> DesenfileirarAsync(CancellationToken cancellationToken = default);
    Task MarcarComoProcessadaAsync(Guid mensagemId);
    Task MarcarComoErroAsync(Guid mensagemId, string erroMensagem, string? stackTrace = null);
    Task MoverParaDeadLetterAsync(Guid mensagemId, string erroMensagem, string? stackTrace = null);
}
