using Microsoft.AspNetCore.Mvc;
using EvaAgent.Aplicacao.DTOs.Mensagens;
using EvaAgent.Aplicacao.Services;

namespace EvaAgent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly OrquestradorMensagensService _orquestrador;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        OrquestradorMensagensService orquestrador,
        ILogger<WebhookController> logger)
    {
        _orquestrador = orquestrador;
        _logger = logger;
    }

    /// <summary>
    /// Recebe mensagens de webhooks de canais externos (WhatsApp, Telegram, etc)
    /// </summary>
    [HttpPost("{espacoId}/mensagem")]
    public async Task<IActionResult> ReceberMensagem(
        Guid espacoId,
        [FromBody] MensagemWebhookDto mensagem)
    {
        _logger.LogInformation("Webhook recebido para espaço {EspacoId} de {Remetente}",
            espacoId, mensagem.RemetenteIdentificador);

        var resposta = await _orquestrador.ProcessarMensagemAsync(espacoId, mensagem);

        if (!resposta.Sucesso)
        {
            return BadRequest(resposta);
        }

        return Ok(resposta);
    }

    /// <summary>
    /// Endpoint de verificação para webhooks do WhatsApp/Facebook
    /// </summary>
    [HttpGet("whatsapp/verify")]
    public IActionResult VerificarWebhookWhatsApp([FromQuery] string hub_mode, [FromQuery] string hub_verify_token, [FromQuery] string hub_challenge)
    {
        const string VERIFY_TOKEN = "agenteia_verify_token_change_this";

        if (hub_mode == "subscribe" && hub_verify_token == VERIFY_TOKEN)
        {
            _logger.LogInformation("Webhook do WhatsApp verificado com sucesso");
            return Content(hub_challenge);
        }

        _logger.LogWarning("Falha na verificação do webhook do WhatsApp");
        return Forbid();
    }

    /// <summary>
    /// Endpoint para receber mensagens do WhatsApp
    /// </summary>
    [HttpPost("whatsapp/{espacoId}")]
    public async Task<IActionResult> ReceberMensagemWhatsApp(
        Guid espacoId,
        [FromBody] dynamic payload)
    {
        // Aqui seria o parsing do payload específico do WhatsApp
        // Simplificado para demonstração
        var mensagem = new MensagemWebhookDto
        {
            CanalTipo = "WhatsApp",
            RemetenteIdentificador = "demo_user",
            Conteudo = "Mensagem de demonstração do WhatsApp",
            RecebidaEm = DateTime.UtcNow
        };

        var resposta = await _orquestrador.ProcessarMensagemAsync(espacoId, mensagem);
        return Ok(new { success = resposta.Sucesso });
    }
}
