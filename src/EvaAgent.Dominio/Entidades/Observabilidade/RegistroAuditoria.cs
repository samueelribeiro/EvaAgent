namespace EvaAgent.Dominio.Entidades.Observabilidade;

public class RegistroAuditoria : EntidadeBase
{
    public Guid? UsuarioId { get; set; }
    public Guid? EspacoId { get; set; }
    public string Entidade { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public Guid? EntidadeId { get; set; }
    public string? ValoresAntigos { get; set; } // JSON
    public string? ValoresNovos { get; set; } // JSON
    public string? IpOrigem { get; set; }
    public string? UserAgent { get; set; }
    public DateTime ExecutadoEm { get; set; }

    // Navegação
    public virtual Identidade.Usuario? Usuario { get; set; }
    public virtual Identidade.Espaco? Espaco { get; set; }
}
