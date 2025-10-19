using EvaAgent.Dominio.Entidades.Memoria;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.Memoria;

/// <summary>
/// Serviço para gerenciamento de memória de curto e longo prazo
/// </summary>
public class MemoriaService : IMemoriaService
{
    private readonly IRepositorioBase<MemoriaCurtoPrazo> _memoriaCurtoRepo;
    private readonly IRepositorioBase<MemoriaLongoPrazo> _memoriaLongoRepo;
    private readonly ILogger<MemoriaService> _logger;

    public MemoriaService(
        IRepositorioBase<MemoriaCurtoPrazo> memoriaCurtoRepo,
        IRepositorioBase<MemoriaLongoPrazo> memoriaLongoRepo,
        ILogger<MemoriaService> logger)
    {
        _memoriaCurtoRepo = memoriaCurtoRepo;
        _memoriaLongoRepo = memoriaLongoRepo;
        _logger = logger;
    }

    public async Task ArmazenarCurtoPrazoAsync(
        Guid conversaId,
        string chave,
        string valor,
        TimeSpan? expiracao = null)
    {
        try
        {
            var expiraEm = expiracao.HasValue
                ? DateTime.UtcNow.Add(expiracao.Value)
                : DateTime.UtcNow.AddHours(24);

            var memoria = new MemoriaCurtoPrazo
            {
                ConversaId = conversaId,
                Chave = chave,
                Valor = valor,
                ExpiraEm = expiraEm
            };

            await _memoriaCurtoRepo.AdicionarAsync(memoria);
            _logger.LogDebug("Memória de curto prazo armazenada - Conversa: {ConversaId}, Chave: {Chave}", conversaId, chave);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao armazenar memória de curto prazo");
            throw;
        }
    }

    public async Task<string?> ObterCurtoPrazoAsync(Guid conversaId, string chave)
    {
        try
        {
            var memorias = await _memoriaCurtoRepo.BuscarAsync(
                m => m.ConversaId == conversaId && m.Chave == chave && m.ExpiraEm > DateTime.UtcNow);

            return memorias.OrderByDescending(m => m.CriadoEm).FirstOrDefault()?.Valor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter memória de curto prazo");
            return null;
        }
    }

    public async Task RemoverCurtoPrazoAsync(Guid conversaId, string chave)
    {
        try
        {
            var memorias = await _memoriaCurtoRepo.BuscarAsync(
                m => m.ConversaId == conversaId && m.Chave == chave);

            foreach (var memoria in memorias)
            {
                await _memoriaCurtoRepo.RemoverAsync(memoria.Id);
            }

            _logger.LogDebug("Memória de curto prazo removida - Conversa: {ConversaId}, Chave: {Chave}", conversaId, chave);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover memória de curto prazo");
            throw;
        }
    }

    public async Task ArmazenarLongoPrazoAsync(
        Guid receptorId,
        string chave,
        string valor,
        string? categoria = null,
        int importancia = 0)
    {
        try
        {
            var memoria = new MemoriaLongoPrazo
            {
                ReceptorId = receptorId,
                Chave = chave,
                Valor = valor,
                Categoria = categoria,
                ImportanciaScore = importancia
            };

            await _memoriaLongoRepo.AdicionarAsync(memoria);
            _logger.LogDebug("Memória de longo prazo armazenada - Receptor: {ReceptorId}, Chave: {Chave}", receptorId, chave);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao armazenar memória de longo prazo");
            throw;
        }
    }

    public async Task<string?> ObterLongoPrazoAsync(Guid receptorId, string chave)
    {
        try
        {
            var memorias = await _memoriaLongoRepo.BuscarAsync(
                m => m.ReceptorId == receptorId && m.Chave == chave);

            return memorias.OrderByDescending(m => m.AtualizadoEm).FirstOrDefault()?.Valor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter memória de longo prazo");
            return null;
        }
    }

    public async Task<Dictionary<string, string>> ObterTodasLongoPrazoAsync(Guid receptorId, string? categoria = null)
    {
        try
        {
            var memorias = await _memoriaLongoRepo.BuscarAsync(m => m.ReceptorId == receptorId);

            return memorias.ToDictionary(m => m.Chave, m => m.Valor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as memórias de longo prazo");
            return new Dictionary<string, string>();
        }
    }

    public async Task RemoverLongoPrazoAsync(Guid receptorId, string chave)
    {
        try
        {
            var memorias = await _memoriaLongoRepo.BuscarAsync(
                m => m.ReceptorId == receptorId && m.Chave == chave);

            foreach (var memoria in memorias)
            {
                await _memoriaLongoRepo.RemoverAsync(memoria.Id);
            }

            _logger.LogDebug("Memória de longo prazo removida - Receptor: {ReceptorId}, Chave: {Chave}", receptorId, chave);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover memória de longo prazo");
            throw;
        }
    }
}
