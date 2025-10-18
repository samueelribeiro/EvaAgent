using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.Canais;

public class Canal : EntidadeBase
{
    public Guid EspacoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoCanal Tipo { get; set; }
    public string? ConfiguracaoJson { get; set; } // Credenciais, tokens, etc.
    public bool Habilitado { get; set; } = true;

    // Navegação
    public virtual Identidade.Espaco Espaco { get; set; } = null!;
    public virtual ICollection<Receptor> Receptores { get; set; } = new List<Receptor>();
}
