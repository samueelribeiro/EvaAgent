namespace EvaAgent.Dominio.Entidades.Observabilidade;

public class RegistroErro : EntidadeBase
{
    public Guid? EspacoId { get; set; }
    public string? CodigoCorrelacao { get; set; }
    public string TipoErro { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? ContextoJson { get; set; }
    public int Severidade { get; set; } // 1=Low, 2=Medium, 3=High, 4=Critical
    public DateTime OcorridoEm { get; set; }
    public bool Resolvido { get; set; }

    // Navegação
    public virtual Identidade.Espaco? Espaco { get; set; }
}
