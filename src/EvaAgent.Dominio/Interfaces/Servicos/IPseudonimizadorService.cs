namespace EvaAgent.Dominio.Interfaces.Servicos;

public interface IPseudonimizadorService
{
    Task<string> PseudonimizarAsync(string texto, Guid? conversaId = null, Guid? solicitacaoIAId = null);
    Task<string> ReverterPseudonimizacaoAsync(string textoPseudonimizado, Guid? conversaId = null, Guid? solicitacaoIAId = null);
    Task<Dictionary<string, string>> ObterMapaPseudonimizacaoAsync(Guid? conversaId = null, Guid? solicitacaoIAId = null);
    Task LimparPseudonimizacoesExpiradasAsync();
}
