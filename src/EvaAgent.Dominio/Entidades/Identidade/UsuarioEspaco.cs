namespace EvaAgent.Dominio.Entidades.Identidade;

public class UsuarioEspaco : EntidadeBase
{
    public Guid UsuarioId { get; set; }
    public Guid EspacoId { get; set; }
    public Guid PapelId { get; set; }
    public DateTime? ConvidadoEm { get; set; }
    public DateTime? AceitoEm { get; set; }

    // Navegação
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Espaco Espaco { get; set; } = null!;
    public virtual Papel Papel { get; set; } = null!;
}
