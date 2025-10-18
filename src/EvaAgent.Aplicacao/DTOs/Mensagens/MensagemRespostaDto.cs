namespace EvaAgent.Aplicacao.DTOs.Mensagens;

public class MensagemRespostaDto
{
    public string Conteudo { get; set; } = string.Empty;
    public bool Sucesso { get; set; }
    public string? Erro { get; set; }
    public Guid? ConversaId { get; set; }
    public Guid? MensagemId { get; set; }
}
