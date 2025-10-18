using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.IA;

public class ProvedorIA : EntidadeBase
{
    public Guid EspacoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoProvedor Tipo { get; set; }
    public string? UrlBase { get; set; }
    public string? ChaveApi { get; set; } // Cifrada
    public string? Modelo { get; set; } // gpt-4, claude-3, gemini-pro, etc.
    public int? MaxTokens { get; set; }
    public decimal? Temperatura { get; set; }
    public bool Habilitado { get; set; } = false;
    public int? LimiteRequisicoesPorMinuto { get; set; }

    // Navegação
    public virtual Identidade.Espaco Espaco { get; set; } = null!;
}
