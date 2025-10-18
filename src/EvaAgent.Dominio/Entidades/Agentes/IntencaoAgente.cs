namespace EvaAgent.Dominio.Entidades.Agentes;

public class IntencaoAgente : EntidadeBase
{
    public Guid AgenteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string PalavrasChaveJson { get; set; } = string.Empty; // Array JSON
    public string? ExemplosJson { get; set; }
    public decimal Confianca { get; set; } = 0.7m; // Threshold de confiança

    // Navegação
    public virtual Agente Agente { get; set; } = null!;
}
