namespace EvaAgent.Dominio.Entidades.Conectores;

public class AcaoNegocio : EntidadeBase
{
    public Guid SistemaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? EndpointUrl { get; set; }
    public string MetodoHttp { get; set; } = "POST";
    public string? ScriptSql { get; set; }
    public string? ParametrosJson { get; set; }
    public bool RequererConfirmacao { get; set; } = true;
    public bool RequererAutenticacao { get; set; } = true;

    // Navegação
    public virtual Sistema Sistema { get; set; } = null!;
}
