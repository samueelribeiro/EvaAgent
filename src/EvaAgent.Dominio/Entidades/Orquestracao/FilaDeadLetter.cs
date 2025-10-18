namespace EvaAgent.Dominio.Entidades.Orquestracao;

public class FilaDeadLetter : EntidadeBase
{
    public Guid FilaMensagemId { get; set; }
    public string TipoMensagem { get; set; } = string.Empty;
    public string ConteudoJson { get; set; } = string.Empty;
    public string ErroMensagem { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public int TentativasProcessamento { get; set; }
    public DateTime EnviadoDeadLetterEm { get; set; }
}
