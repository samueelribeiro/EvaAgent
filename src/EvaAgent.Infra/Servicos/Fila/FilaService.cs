using EvaAgent.Dominio.Entidades.Orquestracao;
using EvaAgent.Dominio.Enums;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EvaAgent.Infra.Servicos.Fila;

/// <summary>
/// Serviço para gerenciamento de fila de mensagens
/// </summary>
public class FilaService : IFilaService
{
    private readonly IRepositorioBase<FilaMensagem> _filaRepo;
    private readonly IRepositorioBase<FilaDeadLetter> _deadLetterRepo;
    private readonly ILogger<FilaService> _logger;
    private const int MAX_TENTATIVAS = 3;

    public FilaService(
        IRepositorioBase<FilaMensagem> filaRepo,
        IRepositorioBase<FilaDeadLetter> deadLetterRepo,
        ILogger<FilaService> logger)
    {
        _filaRepo = filaRepo;
        _deadLetterRepo = deadLetterRepo;
        _logger = logger;
    }

    public async Task EnfileirarAsync(Guid espacoId, string tipoMensagem, object conteudo)
    {
        try
        {
            var mensagem = new FilaMensagem
            {
                EspacoId = espacoId,
                TipoMensagem = tipoMensagem,
                ConteudoJson = JsonSerializer.Serialize(conteudo),
                Status = StatusMensagem.Recebida,
                TentativasProcessamento = 0
            };

            await _filaRepo.AdicionarAsync(mensagem);

            _logger.LogInformation(
                "Mensagem enfileirada - Tipo: {Tipo}, Espaço: {EspacoId}",
                tipoMensagem,
                espacoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enfileirar mensagem");
            throw;
        }
    }

    public async Task<(Guid Id, string TipoMensagem, string ConteudoJson)?> DesenfileirarAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var agora = DateTime.UtcNow;

            var mensagens = await _filaRepo.BuscarAsync(
                m => m.Status == StatusMensagem.Recebida &&
                     m.TentativasProcessamento < m.MaxTentativas,
                cancellationToken);

            var mensagem = mensagens
                .OrderBy(m => m.CriadoEm)
                .FirstOrDefault();

            if (mensagem == null)
                return null;

            // Marcar como processando
            mensagem.Status = StatusMensagem.Processando;
            mensagem.ProcessadoEm = DateTime.UtcNow;
            mensagem.TentativasProcessamento++;

            await _filaRepo.AtualizarAsync(mensagem, cancellationToken);

            return (mensagem.Id, mensagem.TipoMensagem, mensagem.ConteudoJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desenfileirar mensagem");
            return null;
        }
    }

    public async Task MarcarComoProcessadaAsync(Guid mensagemId)
    {
        try
        {
            var mensagem = await _filaRepo.ObterPorIdAsync(mensagemId);
            if (mensagem == null)
                return;

            mensagem.Status = StatusMensagem.Enviada;
            mensagem.ProcessadoEm = DateTime.UtcNow;

            await _filaRepo.AtualizarAsync(mensagem);

            _logger.LogInformation("Mensagem processada com sucesso - ID: {Id}", mensagemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao marcar mensagem como processada");
            throw;
        }
    }

    public async Task MarcarComoErroAsync(Guid mensagemId, string erroMensagem, string? stackTrace = null)
    {
        try
        {
            var mensagem = await _filaRepo.ObterPorIdAsync(mensagemId);
            if (mensagem == null)
                return;

            mensagem.ErroMensagem = erroMensagem;

            if (mensagem.TentativasProcessamento >= mensagem.MaxTentativas)
            {
                await MoverParaDeadLetterAsync(mensagemId, erroMensagem, stackTrace);
            }
            else
            {
                // Reagendar com status recebida
                mensagem.Status = StatusMensagem.Recebida;
                await _filaRepo.AtualizarAsync(mensagem);

                _logger.LogWarning(
                    "Mensagem falhou e foi reagendada - ID: {Id}, Tentativa: {Tentativa}/{Max}",
                    mensagemId,
                    mensagem.TentativasProcessamento,
                    mensagem.MaxTentativas);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao marcar mensagem como erro");
            throw;
        }
    }

    public async Task MoverParaDeadLetterAsync(Guid mensagemId, string erroMensagem, string? stackTrace = null)
    {
        try
        {
            var mensagem = await _filaRepo.ObterPorIdAsync(mensagemId);
            if (mensagem == null)
                return;

            var deadLetter = new FilaDeadLetter
            {
                FilaMensagemId = mensagem.Id,
                TipoMensagem = mensagem.TipoMensagem,
                ConteudoJson = mensagem.ConteudoJson,
                TentativasProcessamento = mensagem.TentativasProcessamento,
                ErroMensagem = erroMensagem,
                StackTrace = stackTrace,
                EnviadoDeadLetterEm = DateTime.UtcNow
            };

            await _deadLetterRepo.AdicionarAsync(deadLetter);
            await _filaRepo.RemoverAsync(mensagem.Id);

            _logger.LogError(
                "Mensagem movida para dead letter queue - Tipo: {Tipo}, Erro: {Erro}",
                mensagem.TipoMensagem,
                erroMensagem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao mover mensagem para dead letter queue");
            throw;
        }
    }
}
