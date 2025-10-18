namespace EvaAgent.Dominio.Entidades.Conversas;

public class Conversa : EntidadeBase
{
    public Guid ReceptorId { get; set; }
    public Guid? AgenteId { get; set; }
    public string? Titulo { get; set; }
    public DateTime IniciadaEm { get; set; }
    public DateTime? FinalizadaEm { get; set; }
    public string? ResumoJson { get; set; }
    public bool Arquivada { get; set; }

    // Navegação
    public virtual Canais.Receptor Receptor { get; set; } = null!;
    public virtual Agentes.Agente? Agente { get; set; }
    public virtual ICollection<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
}
