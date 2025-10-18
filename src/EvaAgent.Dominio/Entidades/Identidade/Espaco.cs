namespace EvaAgent.Dominio.Entidades.Identidade;

public class Espaco : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Slug { get; set; }
    public string? LogoUrl { get; set; }
    public Guid? EspacoPaiId { get; set; }

    // Navegação
    public virtual Espaco? EspacoPai { get; set; }
    public virtual ICollection<Espaco> SubEspacos { get; set; } = new List<Espaco>();
    public virtual ICollection<UsuarioEspaco> UsuarioEspacos { get; set; } = new List<UsuarioEspaco>();
}
