using EvaAgent.Dominio.Enums;

namespace EvaAgent.Dominio.Entidades.Orquestracao;

public class TarefaAgendada : EntidadeBase
{
    public Guid EspacoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string TipoTarefa { get; set; } = string.Empty;
    public string? ParametrosJson { get; set; }
    public string? CronExpressao { get; set; }
    public DateTime? ProximaExecucao { get; set; }
    public DateTime? UltimaExecucao { get; set; }
    public StatusTarefa Status { get; set; }
    public bool Habilitada { get; set; } = true;

    // Navegação
    public virtual Identidade.Espaco Espaco { get; set; } = null!;
}
