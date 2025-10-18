namespace EvaAgent.Dominio.Entidades.IA;

public class SolicitacaoIA : EntidadeBase
{
    public Guid ProvedorIAId { get; set; }
    public Guid? ConversaId { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public string? ContextoJson { get; set; } // Histórico, memória, etc.
    public int? TokensPrompt { get; set; }
    public DateTime SolicitadoEm { get; set; }

    // Navegação
    public virtual ProvedorIA ProvedorIA { get; set; } = null!;
    public virtual Conversas.Conversa? Conversa { get; set; }
    public virtual RespostaIA? Resposta { get; set; }
}
