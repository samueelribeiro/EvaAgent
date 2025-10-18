namespace EvaAgent.Dominio.Entidades.Conectores;

public class Sistema : EntidadeBase
{
    public Guid EspacoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? VersaoApi { get; set; }

    // Navegação
    public virtual Identidade.Espaco Espaco { get; set; } = null!;
    public virtual ICollection<Conector> Conectores { get; set; } = new List<Conector>();
    public virtual ICollection<ConsultaNegocio> Consultas { get; set; } = new List<ConsultaNegocio>();
    public virtual ICollection<AcaoNegocio> Acoes { get; set; } = new List<AcaoNegocio>();
}
