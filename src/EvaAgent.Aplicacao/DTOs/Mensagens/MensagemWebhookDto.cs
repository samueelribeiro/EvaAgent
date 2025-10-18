namespace EvaAgent.Aplicacao.DTOs.Mensagens;

public class MensagemWebhookDto
{
    public string CanalTipo { get; set; } = string.Empty; // whatsapp, telegram, email, etc
    public string RemetenteIdentificador { get; set; } = string.Empty;
    public string? RemetenteNome { get; set; }
    public string Conteudo { get; set; } = string.Empty;
    public string? IdExterno { get; set; }
    public DateTime RecebidaEm { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string>? Metadados { get; set; }
}
