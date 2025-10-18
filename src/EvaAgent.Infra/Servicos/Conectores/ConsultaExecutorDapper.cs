using Dapper;
using EvaAgent.Dominio.Entidades.Conectores;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;
using EvaAgent.Dominio.Enums;
using Microsoft.Extensions.Logging;

namespace EvaAgent.Infra.Servicos.Conectores;

/// <summary>
/// Executor de consultas SQL usando Dapper
/// </summary>
public class ConsultaExecutorDapper
{
    private readonly IRepositorioBase<ConsultaNegocio> _repoConsulta;
    private readonly IRepositorioBase<Sistema> _repoSistema;
    private readonly DbConnectionFactory _connectionFactory;
    private readonly ICryptoService _cryptoService;
    private readonly ILogger<ConsultaExecutorDapper> _logger;

    public ConsultaExecutorDapper(
        IRepositorioBase<ConsultaNegocio> repoConsulta,
        IRepositorioBase<Sistema> repoSistema,
        DbConnectionFactory connectionFactory,
        ICryptoService cryptoService,
        ILogger<ConsultaExecutorDapper> logger)
    {
        _repoConsulta = repoConsulta;
        _repoSistema = repoSistema;
        _connectionFactory = connectionFactory;
        _cryptoService = cryptoService;
        _logger = logger;
    }

    /// <summary>
    /// Executa uma consulta de negócio
    /// </summary>
    public async Task<IEnumerable<dynamic>> ExecutarAsync(
        Guid consultaId,
        Dictionary<string, object>? parametros = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Buscar consulta
            var consulta = await _repoConsulta.ObterPorIdAsync(consultaId, cancellationToken);
            if (consulta == null)
                throw new ArgumentException($"Consulta {consultaId} não encontrada");

            // Buscar sistema e conector
            var sistema = await _repoSistema.ObterPorIdAsync(consulta.SistemaId, cancellationToken);
            if (sistema == null)
                throw new InvalidOperationException("Sistema não encontrado");

            var conector = sistema.Conectores
                .FirstOrDefault(c => c.Tipo == TipoConector.BancoDados && c.Habilitado);

            if (conector == null)
                throw new InvalidOperationException("Nenhum conector de banco habilitado");

            // Descriptografar connection string
            var connectionString = _cryptoService.Descriptografar(conector.StringConexao!);

            _logger.LogInformation(
                "Executando consulta {Consulta} no sistema {Sistema}",
                consulta.Nome,
                sistema.Nome);

            // Criar conexão e executar
            using var connection = _connectionFactory.CriarConexao(
                conector.TipoBancoDados!.Value,
                connectionString);

            connection.Open();

            var resultado = await connection.QueryAsync(
                consulta.QuerySql,
                parametros,
                commandTimeout: conector.TimeoutSegundos ?? 30);

            stopwatch.Stop();

            _logger.LogInformation(
                "Consulta executada com sucesso em {Tempo}ms - {Linhas} linhas retornadas",
                stopwatch.ElapsedMilliseconds,
                resultado.Count());

            return resultado;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "Erro ao executar consulta {ConsultaId} após {Tempo}ms",
                consultaId,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    /// <summary>
    /// Executa uma consulta SQL direta (use com cuidado)
    /// </summary>
    public async Task<IEnumerable<dynamic>> ExecutarSqlDiretoAsync(
        Guid sistemaId,
        string sql,
        Dictionary<string, object>? parametros = null,
        CancellationToken cancellationToken = default)
    {
        var sistema = await _repoSistema.ObterPorIdAsync(sistemaId, cancellationToken);
        if (sistema == null)
            throw new ArgumentException("Sistema não encontrado");

        var conector = sistema.Conectores
            .FirstOrDefault(c => c.Tipo == TipoConector.BancoDados && c.Habilitado);

        if (conector == null)
            throw new InvalidOperationException("Nenhum conector de banco habilitado");

        var connectionString = _cryptoService.Descriptografar(conector.StringConexao!);

        using var connection = _connectionFactory.CriarConexao(
            conector.TipoBancoDados!.Value,
            connectionString);

        connection.Open();

        return await connection.QueryAsync(sql, parametros);
    }
}
