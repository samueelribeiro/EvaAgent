using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.Identidade;

public class Papel : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoPapel Tipo { get; set; }
    public string? Permissoes { get; set; } // JSON array de permissões

    // Navegação
    public virtual ICollection<UsuarioEspaco> UsuarioEspacos { get; set; } = new List<UsuarioEspaco>();
}
