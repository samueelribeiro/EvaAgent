namespace EvaAgent.Dominio.Entidades.Memoria;

public class MemoriaCurtoPrazo : EntidadeBase
{
    public Guid ConversaId { get; set; }
    public string Chave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }

    // Navegação
    public virtual Conversas.Conversa Conversa { get; set; } = null!;
}
