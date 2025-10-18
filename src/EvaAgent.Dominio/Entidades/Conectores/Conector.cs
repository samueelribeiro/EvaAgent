using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.Conectores;

public class Conector : EntidadeBase
{
    public Guid SistemaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoConector Tipo { get; set; }

    // Para API REST
    public string? UrlBase { get; set; }
    public string? ChaveApi { get; set; } // Cifrada
    public string? HeadersJson { get; set; }

    // Para Banco de Dados
    public TipoBancoDados? TipoBancoDados { get; set; }
    public string? StringConexao { get; set; } // Cifrada

    public bool Habilitado { get; set; } = true;
    public int? TimeoutSegundos { get; set; } = 30;

    // Navegação
    public virtual Sistema Sistema { get; set; } = null!;
}
