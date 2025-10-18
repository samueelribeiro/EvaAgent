using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.Conversas;

public class Mensagem : EntidadeBase
{
    public Guid ConversaId { get; set; }
    public DirecaoMensagem Direcao { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public StatusMensagem Status { get; set; }
    public DateTime EnviadaEm { get; set; }
    public DateTime? EntregueEm { get; set; }
    public DateTime? LidaEm { get; set; }
    public string? MidiaUrl { get; set; }
    public string? TipoMidia { get; set; }
    public string? MetadadosJson { get; set; }
    public string? IdExterno { get; set; } // ID da mensagem no canal externo

    // Navegação
    public virtual Conversa Conversa { get; set; } = null!;
}
