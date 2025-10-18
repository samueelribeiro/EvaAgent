namespace EvaAgent.Dominio.Entidades;

/// <summary>
/// Classe base para todas as entidades do domínio
/// </summary>
public abstract class EntidadeBase
{
    public Guid Id { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public bool Ativo { get; set; } = true;

    protected EntidadeBase()
    {
        Id = Guid.NewGuid();
        CriadoEm = DateTime.UtcNow;
    }
}
