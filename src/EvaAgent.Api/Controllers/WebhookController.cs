using Microsoft.AspNetCore.Mvc;
using EvaAgent.Aplicacao.DTOs.Mensagens;
using EvaAgent.Aplicacao.Services;

namespace EvaAgent.Api.Controllers;

/// <summary>
/// Controller para receber mensagens de canais externos via webhooks
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
    /// Recebe mensagens de webhooks de canais externos
    /// </summary>
    /// <remarks>
    /// Este endpoint recebe mensagens de diferentes canais de comunicação (WhatsApp, Telegram, WebChat, Email, SMS).
    ///
    /// **Fluxo de Processamento:**
    /// 1. Identifica/cria o receptor
    /// 2. Cria/recupera a conversa
    /// 3. Pseudonimiza dados sensíveis (LGPD)
    /// 4. Resolve a intenção e seleciona o agente apropriado
    /// 5. Processa a mensagem com o agente especializado
    /// 6. Reverte a pseudonimização
    /// 7. Retorna a resposta formatada
    ///
    /// **Exemplo de uso:**
    /// ```json
    /// {
    ///   "canalTipo": "WhatsApp",
    ///   "remetenteIdentificador": "5511999999999",
    ///   "remetenteNome": "João Silva",
    ///   "conteudo": "Qual foi o valor das vendas de hoje?",
    ///   "recebidaEm": "2025-10-18T14:30:00Z"
    /// }
    /// ```
    /// </remarks>
    /// <param name="espacoId">ID do espaço (tenant) que processará a mensagem</param>
    /// <param name="mensagem">Dados da mensagem recebida</param>
    /// <returns>Resposta processada pela IA</returns>
    /// <response code="200">Mensagem processada com sucesso</response>
    /// <response code="400">Erro no processamento da mensagem</response>
    [HttpPost("{espacoId}/mensagem")]
    [ProducesResponseType(typeof(MensagemRespostaDto), 200)]
    [ProducesResponseType(typeof(MensagemRespostaDto), 400)]
    public async Task<IActionResult> ReceberMensagem(
        [FromRoute] Guid espacoId,
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
    /// Endpoint de verificação para webhooks do WhatsApp Business API
    /// </summary>
    /// <remarks>
    /// O Facebook/WhatsApp Business requer um endpoint de verificação inicial.
    /// Este endpoint responde com o `hub_challenge` se o token estiver correto.
    ///
    /// **Configuração:**
    /// - Configure o token de verificação no arquivo de configuração
    /// - Forneça esta URL ao configurar o webhook no painel do WhatsApp Business
    /// </remarks>
    /// <param name="hub_mode">Modo do hub (deve ser "subscribe")</param>
    /// <param name="hub_verify_token">Token de verificação configurado</param>
    /// <param name="hub_challenge">Challenge fornecido pelo Facebook</param>
    /// <returns>Challenge se verificação bem-sucedida</returns>
    /// <response code="200">Verificação bem-sucedida</response>
    /// <response code="403">Token de verificação inválido</response>
    [HttpGet("whatsapp/verify")]
    [ProducesResponseType(200)]
    [ProducesResponseType(403)]
    public IActionResult VerificarWebhookWhatsApp(
        [FromQuery] string hub_mode,
        [FromQuery] string hub_verify_token,
        [FromQuery] string hub_challenge)
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
    /// Recebe mensagens do WhatsApp Business API
    /// </summary>
    /// <remarks>
    /// Este endpoint recebe mensagens diretamente do WhatsApp Business API.
    /// O payload é específico do formato WhatsApp e será parseado internamente.
    ///
    /// **Nota:** O payload real do WhatsApp é complexo e contém metadados adicionais.
    /// Consulte a documentação oficial do WhatsApp Business API para detalhes.
    /// </remarks>
    /// <param name="espacoId">ID do espaço que processará a mensagem</param>
    /// <param name="payload">Payload completo do WhatsApp Business API</param>
    /// <returns>Confirmação de recebimento</returns>
    /// <response code="200">Mensagem recebida e processada</response>
    [HttpPost("whatsapp/{espacoId}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ReceberMensagemWhatsApp(
        [FromRoute] Guid espacoId,
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
