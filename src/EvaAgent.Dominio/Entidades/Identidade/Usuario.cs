using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.Identidade;

public class Usuario : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Avatar { get; set; }
    public string? TimeZone { get; set; }
    public string? Idioma { get; set; }
    public DateTime? UltimoAcesso { get; set; }
    public bool EmailVerificado { get; set; }

    // Navegação
    public virtual ICollection<UsuarioEspaco> UsuarioEspacos { get; set; } = new List<UsuarioEspaco>();
}
