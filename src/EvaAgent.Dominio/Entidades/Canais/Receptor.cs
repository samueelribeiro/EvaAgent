using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.Canais;

public class Receptor : EntidadeBase
{
    public Guid CanalId { get; set; }
    public string Identificador { get; set; } = string.Empty; // número de telefone, email, user_id, etc.
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public string? MetadadosJson { get; set; }

    // Preferências de Tom
    public TomAtendimento TomAtendimento { get; set; } = TomAtendimento.Profissional;
    public TipoFormatoNome FormatoNome { get; set; } = TipoFormatoNome.PrimeiroNome;
    public bool UsarSaudacao { get; set; } = true;
    public string? Idioma { get; set; }
    public string? TimeZone { get; set; }

    // Navegação
    public virtual Canal Canal { get; set; } = null!;
    public virtual ICollection<Conversas.Conversa> Conversas { get; set; } = new List<Conversas.Conversa>();
}
