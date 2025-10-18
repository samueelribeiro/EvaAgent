namespace EvaAgent.Dominio.Entidades.Conectores;

public class ConsultaNegocio : EntidadeBase
{
    public Guid SistemaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string QuerySql { get; set; } = string.Empty;
    public string? ParametrosJson { get; set; } // Schema dos parâmetros esperados
    public bool RequererAutenticacao { get; set; } = true;

    // Navegação
    public virtual Sistema Sistema { get; set; } = null!;
}
