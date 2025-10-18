using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.Agentes;

public class Agente : EntidadeBase
{
    public Guid EspacoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoAgente Tipo { get; set; }
    public string? PromptSistema { get; set; }
    public string? PalavrasChaveJson { get; set; } // Array JSON de palavras-chave para roteamento
    public bool Habilitado { get; set; } = true;
    public int Prioridade { get; set; } = 0;

    // Navegação
    public virtual Identidade.Espaco Espaco { get; set; } = null!;
    public virtual ICollection<IntencaoAgente> Intencoes { get; set; } = new List<IntencaoAgente>();
}
