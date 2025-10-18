namespace EvaAgent.Dominio.Entidades.LGPD;

public class ConsentimentoLGPD : EntidadeBase
{
    public Guid ReceptorId { get; set; }
    public string Finalidade { get; set; } = string.Empty;
    public bool Consentido { get; set; }
    public DateTime ConsentidoEm { get; set; }
    public DateTime? RevogadoEm { get; set; }
    public string? IpOrigem { get; set; }

    // Navegação
    public virtual Canais.Receptor Receptor { get; set; } = null!;
}
