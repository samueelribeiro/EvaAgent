using Microsoft.EntityFrameworkCore;
using EvaAgent.Dominio.Entidades;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Infra.Data.Contexts;
using System.Linq.Expressions;

namespace EvaAgent.Infra.Data.Repositories;

public class RepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public RepositorioBase(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.Id == id && e.Ativo)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.Ativo)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> BuscarAsync(
        Expression<Func<T, bool>> predicado,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.Ativo)
            .Where(predicado)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<T> AdicionarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        entidade.CriadoEm = DateTime.UtcNow;
        entidade.Ativo = true;

        await _dbSet.AddAsync(entidade, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entidade;
    }

    public virtual async Task AtualizarAsync(T entidade, CancellationToken cancellationToken = default)
    {
        entidade.AtualizadoEm = DateTime.UtcNow;

        _dbSet.Update(entidade);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entidade = await ObterPorIdAsync(id, cancellationToken);
        if (entidade != null)
        {
            // Soft delete
            entidade.Ativo = false;
            entidade.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id == id && e.Ativo, cancellationToken);
    }

    public virtual async Task<int> ContarAsync(
        Expression<Func<T, bool>>? predicado = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => e.Ativo);

        if (predicado != null)
        {
            query = query.Where(predicado);
        }

        return await query.CountAsync(cancellationToken);
    }
}
