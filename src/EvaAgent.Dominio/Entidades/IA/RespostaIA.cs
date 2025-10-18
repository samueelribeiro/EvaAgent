namespace EvaAgent.Dominio.Entidades.IA;

public class RespostaIA : EntidadeBase
{
    public Guid SolicitacaoIAId { get; set; }
    public string Resposta { get; set; } = string.Empty;
    public int? TokensResposta { get; set; }
    public decimal? CustoEstimado { get; set; }
    public int? TempoRespostaMs { get; set; }
    public DateTime RespondidoEm { get; set; }
    public string? MetadadosJson { get; set; }

    // Navegação
    public virtual SolicitacaoIA SolicitacaoIA { get; set; } = null!;
}
