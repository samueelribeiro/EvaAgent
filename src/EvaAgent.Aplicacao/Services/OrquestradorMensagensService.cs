using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EvaAgent.Aplicacao.DTOs.Mensagens;
using EvaAgent.Dominio.Entidades.Canais;
using EvaAgent.Dominio.Entidades.Conversas;
using EvaAgent.Dominio.Entidades.Agentes;
using EvaAgent.Dominio.Enums;
using EvaAgent.Dominio.Interfaces.Servicos;
using EvaAgent.Infra.Data.Contexts;

namespace EvaAgent.Aplicacao.Services;

public class OrquestradorMensagensService
{
    private readonly AppDbContext _context;
    private readonly IIntentResolverService _intentResolver;
    private readonly IPseudonimizadorService _pseudonimizador;
    private readonly ILogger<OrquestradorMensagensService> _logger;

    public OrquestradorMensagensService(
        AppDbContext context,
        IIntentResolverService intentResolver,
        IPseudonimizadorService pseudonimizador,
        ILogger<OrquestradorMensagensService> logger)
    {
        _context = context;
        _intentResolver = intentResolver;
        _pseudonimizador = pseudonimizador;
        _logger = logger;
    }

    public async Task<MensagemRespostaDto> ProcessarMensagemAsync(Guid espacoId, MensagemWebhookDto webhookDto)
    {
        try
        {
            _logger.LogInformation("Processando mensagem de {Remetente} no canal {Canal}",
                webhookDto.RemetenteIdentificador, webhookDto.CanalTipo);

            // 1. Obter ou criar receptor
            var receptor = await ObterOuCriarReceptorAsync(espacoId, webhookDto);

            // 2. Obter ou criar conversa
            var conversa = await ObterOuCriarConversaAsync(receptor.Id);

            // 3. Salvar mensagem recebida
            var mensagem = new Mensagem
            {
                ConversaId = conversa.Id,
                Direcao = DirecaoMensagem.Entrada,
                Conteudo = webhookDto.Conteudo,
                Status = StatusMensagem.Recebida,
                EnviadaEm = webhookDto.RecebidaEm,
                IdExterno = webhookDto.IdExterno
            };

            _context.Mensagens.Add(mensagem);
            await _context.SaveChangesAsync();

            // 4. Pseudonimizar dados sensíveis antes de enviar para IA
            var conteudoPseudonimizado = await _pseudonimizador.PseudonimizarAsync(
                webhookDto.Conteudo,
                conversa.Id);

            // 5. Identificar intenção e agente apropriado
            var agente = await _intentResolver.ResolverAgenteAsync(conteudoPseudonimizado, espacoId);

            if (agente != null && conversa.AgenteId != agente.Id)
            {
                conversa.AgenteId = agente.Id;
                _context.Conversas.Update(conversa);
                await _context.SaveChangesAsync();
            }

            // 6. Processar com o agente (simplificado - seria chamada real ao agente)
            var respostaAgente = await ProcessarComAgenteAsync(agente, conteudoPseudonimizado, conversa, receptor);

            // 7. Reverter pseudonimização na resposta
            var respostaFinal = await _pseudonimizador.ReverterPseudonimizacaoAsync(respostaAgente, conversa.Id);

            // 8. Salvar resposta
            var mensagemResposta = new Mensagem
            {
                ConversaId = conversa.Id,
                Direcao = DirecaoMensagem.Saida,
                Conteudo = respostaFinal,
                Status = StatusMensagem.Enviada,
                EnviadaEm = DateTime.UtcNow
            };

            _context.Mensagens.Add(mensagemResposta);
            await _context.SaveChangesAsync();

            return new MensagemRespostaDto
            {
                Conteudo = respostaFinal,
                Sucesso = true,
                ConversaId = conversa.Id,
                MensagemId = mensagemResposta.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem");
            return new MensagemRespostaDto
            {
                Conteudo = "Desculpe, ocorreu um erro ao processar sua mensagem. Por favor, tente novamente.",
                Sucesso = false,
                Erro = ex.Message
            };
        }
    }

    private async Task<Receptor> ObterOuCriarReceptorAsync(Guid espacoId, MensagemWebhookDto webhookDto)
    {
        // Buscar canal do espaço
        var tipoCanal = Enum.Parse<TipoCanal>(webhookDto.CanalTipo, true);
        var canal = await _context.Canais
            .FirstOrDefaultAsync(c => c.EspacoId == espacoId && c.Tipo == tipoCanal && c.Habilitado);

        if (canal == null)
        {
            throw new InvalidOperationException($"Canal {webhookDto.CanalTipo} não encontrado ou desabilitado");
        }

        // Buscar ou criar receptor
        var receptor = await _context.Receptores
            .FirstOrDefaultAsync(r => r.CanalId == canal.Id && r.Identificador == webhookDto.RemetenteIdentificador);

        if (receptor == null)
        {
            receptor = new Receptor
            {
                CanalId = canal.Id,
                Identificador = webhookDto.RemetenteIdentificador,
                Nome = webhookDto.RemetenteNome
            };

            _context.Receptores.Add(receptor);
            await _context.SaveChangesAsync();
        }

        return receptor;
    }

    private async Task<Conversa> ObterOuCriarConversaAsync(Guid receptorId)
    {
        // Buscar conversa ativa (não arquivada)
        var conversa = await _context.Conversas
            .Where(c => c.ReceptorId == receptorId && !c.Arquivada)
            .OrderByDescending(c => c.IniciadaEm)
            .FirstOrDefaultAsync();

        if (conversa == null)
        {
            conversa = new Conversa
            {
                ReceptorId = receptorId,
                IniciadaEm = DateTime.UtcNow
            };

            _context.Conversas.Add(conversa);
            await _context.SaveChangesAsync();
        }

        return conversa;
    }

    private async Task<string> ProcessarComAgenteAsync(
        Agente? agente,
        string conteudoPseudonimizado,
        Conversa conversa,
        Receptor receptor)
    {
        if (agente == null)
        {
            return "Olá! Sou um assistente virtual. Como posso ajudá-lo hoje?";
        }

        // Aqui seria a integração real com o agente especializado
        // Por enquanto, retornamos uma resposta simulada
        var saudacao = receptor.UsarSaudacao ? $"Olá, {receptor.Nome ?? ""}! " : "";

        return $"{saudacao}Recebi sua mensagem e vou processar com o agente {agente.Nome}. " +
               "Esta é uma resposta de demonstração. A integração completa com IA será implementada em breve.";
    }
}
