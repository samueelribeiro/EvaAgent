namespace EvaAgent.Dominio.Entidades.LGPD;

public class RegistroPseudonimizacao : EntidadeBase
{
    public Guid Guid { get; set; }
    public string ValorOriginalHash { get; set; } = string.Empty; // SHA256
    public string ValorCifrado { get; set; } = string.Empty; // AES256
    public string TipoDado { get; set; } = string.Empty; // CPF, CNPJ, Nome, Email, etc.
    public Guid? ConversaId { get; set; }
    public Guid? SolicitacaoIAId { get; set; }
    public DateTime PseudonimizadoEm { get; set; }
    public DateTime? RevertidoEm { get; set; }
    public DateTime? ExpiraEm { get; set; }

    // Navegação
    public virtual Conversas.Conversa? Conversa { get; set; }
    public virtual IA.SolicitacaoIA? SolicitacaoIA { get; set; }
}
