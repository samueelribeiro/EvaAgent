namespace EvaAgent.Dominio.Entidades.Observabilidade;

public class MetricaUso : EntidadeBase
{
    public Guid EspacoId { get; set; }
    public string TipoMetrica { get; set; } = string.Empty;
    public string? Recurso { get; set; }
    public decimal Quantidade { get; set; }
    public string? UnidadeMedida { get; set; }
    public string? DimensoesJson { get; set; }
    public DateTime MedidoEm { get; set; }

    // Navegação
    public virtual Identidade.Espaco Espaco { get; set; } = null!;
}
