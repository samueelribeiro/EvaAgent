using EvaAgent.Dominio.Entidades;
using System.Linq.Expressions;

namespace EvaAgent.Dominio.Interfaces.Repositorios;

public interface IRepositorioBase<T> where T : EntidadeBase
{
    Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado, CancellationToken cancellationToken = default);
    Task<T> AdicionarAsync(T entidade, CancellationToken cancellationToken = default);
    Task AtualizarAsync(T entidade, CancellationToken cancellationToken = default);
    Task RemoverAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> ContarAsync(Expression<Func<T, bool>>? predicado = null, CancellationToken cancellationToken = default);
}
