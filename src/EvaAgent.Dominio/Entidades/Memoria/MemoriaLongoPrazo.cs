namespace EvaAgent.Dominio.Entidades.Memoria;

public class MemoriaLongoPrazo : EntidadeBase
{
    public Guid ReceptorId { get; set; }
    public string Chave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public int? ImportanciaScore { get; set; }

    // Navegação
    public virtual Canais.Receptor Receptor { get; set; } = null!;
}
