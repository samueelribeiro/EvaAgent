using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.Orquestracao;

public class FilaMensagem : EntidadeBase
{
    public Guid EspacoId { get; set; }
    public string TipoMensagem { get; set; } = string.Empty;
    public string ConteudoJson { get; set; } = string.Empty;
    public StatusMensagem Status { get; set; }
    public int TentativasProcessamento { get; set; } = 0;
    public int MaxTentativas { get; set; } = 3;
    public DateTime? ProcessadoEm { get; set; }
    public string? ErroMensagem { get; set; }

    // Navegação
    public virtual Identidade.Espaco Espaco { get; set; } = null!;
}
