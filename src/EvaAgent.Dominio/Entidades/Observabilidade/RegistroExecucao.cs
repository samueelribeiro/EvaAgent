namespace EvaAgent.Dominio.Entidades.Observabilidade;

public class RegistroExecucao : EntidadeBase
{
    public Guid EspacoId { get; set; }
    public Guid? ConversaId { get; set; }
    public string TipoExecucao { get; set; } = string.Empty; // Consulta, Ação, etc.
    public Guid? RecursoId { get; set; }
    public string? NomeRecurso { get; set; }
    public string? ParametrosJson { get; set; }
    public string? ResultadoJson { get; set; }
    public bool Sucesso { get; set; }
    public int TempoExecucaoMs { get; set; }
    public DateTime ExecutadoEm { get; set; }

    // Navegação
    public virtual Identidade.Espaco Espaco { get; set; } = null!;
    public virtual Conversas.Conversa? Conversa { get; set; }
}
