namespace EvaAgent.Dominio.Entidades.Memoria;

public class GrupoTreinamento : EntidadeBase
{
    public Guid EspacoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Tags { get; set; } // Comma-separated

    // Navegação
    public virtual Identidade.Espaco Espaco { get; set; } = null!;
    public virtual ICollection<DocumentoTreinamento> Documentos { get; set; } = new List<DocumentoTreinamento>();
}
