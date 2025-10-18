namespace EvaAgent.Dominio.Entidades.Memoria;

public class DocumentoTreinamento : EntidadeBase
{
    public Guid GrupoTreinamentoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string ConteudoOriginal { get; set; } = string.Empty;
    public string? ConteudoProcessado { get; set; }
    public string? Embedding { get; set; } // Vetores
    public string? MetadadosJson { get; set; }
    public DateTime IngeridoEm { get; set; }

    // Navegação
    public virtual GrupoTreinamento GrupoTreinamento { get; set; } = null!;
}
