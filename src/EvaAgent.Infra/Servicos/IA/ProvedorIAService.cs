using EvaAgent.Dominio.Entidades.IA;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.IA;

/// <summary>
/// Serviço para gerenciar e executar solicitações aos provedores de IA
/// </summary>
public class ProvedorIAService
{
    private readonly IRepositorioBase<ProvedorIA> _repositorioProvedor;
    private readonly IRepositorioBase<SolicitacaoIA> _repositorioSolicitacao;
    private readonly IRepositorioBase<RespostaIA> _repositorioResposta;
    private readonly ProvedorIAFactory _factory;
    private readonly ILogger<ProvedorIAService> _logger;

    public ProvedorIAService(
        IRepositorioBase<ProvedorIA> repositorioProvedor,
        IRepositorioBase<SolicitacaoIA> repositorioSolicitacao,
        IRepositorioBase<RespostaIA> repositorioResposta,
        ProvedorIAFactory factory,
        ILogger<ProvedorIAService> logger)
    {
        _repositorioProvedor = repositorioProvedor;
        _repositorioSolicitacao = repositorioSolicitacao;
        _repositorioResposta = repositorioResposta;
        _factory = factory;
        _logger = logger;
    }

    /// <summary>
    /// Executa uma solicitação de IA usando o provedor padrão do espaço
    /// </summary>
    public async Task<RespostaIA> ExecutarSolicitacaoAsync(
        Guid espacoId,
        string prompt,
        string? contexto = null,
        Guid? conversaId = null,
        CancellationToken cancellationToken = default)
    {
        // Buscar provedor habilitado do espaço
        var provedores = await _repositorioProvedor.BuscarAsync(
            p => p.EspacoId == espacoId && p.Habilitado,
            cancellationToken);

        var provedor = provedores.FirstOrDefault();
        if (provedor == null)
        {
            throw new InvalidOperationException(
                $"Nenhum provedor de IA habilitado encontrado para o espaço {espacoId}");
        }

        return await ExecutarSolicitacaoComProvedorAsync(
            provedor.Id,
            prompt,
            contexto,
            conversaId,
            cancellationToken);
    }

    /// <summary>
    /// Executa uma solicitação de IA usando um provedor específico
    /// </summary>
    public async Task<RespostaIA> ExecutarSolicitacaoComProvedorAsync(
        Guid provedorId,
        string prompt,
        string? contexto = null,
        Guid? conversaId = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Buscar configuração do provedor
            var provedorConfig = await _repositorioProvedor.ObterPorIdAsync(provedorId, cancellationToken);
            if (provedorConfig == null)
            {
                throw new ArgumentException($"Provedor {provedorId} não encontrado", nameof(provedorId));
            }

            // Registrar solicitação
            var solicitacao = new SolicitacaoIA
            {
                ProvedorIAId = provedorId,
                ConversaId = conversaId,
                Prompt = prompt,
                ContextoJson = contexto,
                SolicitadoEm = DateTime.UtcNow
            };

            solicitacao = await _repositorioSolicitacao.AdicionarAsync(solicitacao, cancellationToken);

            _logger.LogInformation(
                "Executando solicitação {SolicitacaoId} com provedor {Provedor}",
                solicitacao.Id,
                provedorConfig.Nome);

            // Criar instância do provedor e executar
            var provedorIA = _factory.Criar(provedorConfig);

            var (respostaTexto, tokensPrompt, tokensResposta, custo) =
                await provedorIA.GerarRespostaDetalhadaAsync(prompt, contexto, cancellationToken);

            stopwatch.Stop();

            // Atualizar tokens da solicitação
            solicitacao.TokensPrompt = tokensPrompt;
            await _repositorioSolicitacao.AtualizarAsync(solicitacao, cancellationToken);

            // Registrar resposta
            var resposta = new RespostaIA
            {
                SolicitacaoIAId = solicitacao.Id,
                Resposta = respostaTexto,
                TokensResposta = tokensResposta,
                CustoEstimado = custo,
                TempoRespostaMs = (int)stopwatch.ElapsedMilliseconds,
                RespondidoEm = DateTime.UtcNow
            };

            resposta = await _repositorioResposta.AdicionarAsync(resposta, cancellationToken);

            _logger.LogInformation(
                "Solicitação {SolicitacaoId} concluída - Tokens: {Tokens}, Custo: {Custo:C}, Tempo: {Tempo}ms",
                solicitacao.Id,
                tokensPrompt + tokensResposta,
                custo,
                stopwatch.ElapsedMilliseconds);

            return resposta;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar solicitação de IA");
            throw;
        }
    }

    /// <summary>
    /// Lista todos os provedores de IA de um espaço
    /// </summary>
    public async Task<IEnumerable<ProvedorIA>> ListarProvedoresAsync(
        Guid espacoId,
        bool apenasHabilitados = true,
        CancellationToken cancellationToken = default)
    {
        if (apenasHabilitados)
        {
            return await _repositorioProvedor.BuscarAsync(
                p => p.EspacoId == espacoId && p.Habilitado,
                cancellationToken);
        }

        return await _repositorioProvedor.BuscarAsync(
            p => p.EspacoId == espacoId,
            cancellationToken);
    }

    /// <summary>
    /// Obtém estatísticas de uso de um provedor
    /// </summary>
    public async Task<EstatisticasProvedor> ObterEstatisticasAsync(
        Guid provedorId,
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        CancellationToken cancellationToken = default)
    {
        var inicio = dataInicio ?? DateTime.UtcNow.AddDays(-30);
        var fim = dataFim ?? DateTime.UtcNow;

        var solicitacoes = await _repositorioSolicitacao.BuscarAsync(
            s => s.ProvedorIAId == provedorId &&
                 s.CriadoEm >= inicio &&
                 s.CriadoEm <= fim,
            cancellationToken);

        var respostaIds = solicitacoes.Select(s => s.Id).ToList();
        var todasRespostas = await _repositorioResposta.ObterTodosAsync(cancellationToken);
        var respostas = todasRespostas.Where(r => respostaIds.Contains(r.SolicitacaoIAId)).ToList();

        return new EstatisticasProvedor
        {
            TotalSolicitacoes = solicitacoes.Count(),
            TotalTokensPrompt = solicitacoes.Sum(s => s.TokensPrompt ?? 0),
            TotalTokensResposta = respostas.Sum(r => r.TokensResposta ?? 0),
            CustoTotal = respostas.Sum(r => r.CustoEstimado ?? 0),
            TempoMedioResposta = respostas.Any()
                ? respostas.Average(r => r.TempoRespostaMs ?? 0)
                : 0
        };
    }
}

/// <summary>
/// Estatísticas de uso de um provedor de IA
/// </summary>
public class EstatisticasProvedor
{
    public int TotalSolicitacoes { get; set; }
    public int TotalTokensPrompt { get; set; }
    public int TotalTokensResposta { get; set; }
    public decimal CustoTotal { get; set; }
    public double TempoMedioResposta { get; set; }
}
