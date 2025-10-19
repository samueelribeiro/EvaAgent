using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvaAgent.Infra.Data.Contexts;

namespace EvaAgent.Api.Controllers;

/// <summary>
/// Controller para verificação de saúde da aplicação
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(AppDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Verifica o status geral de saúde da aplicação
    /// </summary>
    /// <remarks>
    /// Retorna o status de saúde da aplicação incluindo conectividade com o banco de dados.
    ///
    /// **Status Possíveis:**
    /// - Healthy: Aplicação funcionando normalmente
    /// - Unhealthy: Aplicação com problemas
    /// </remarks>
    /// <returns>Status de saúde da aplicação</returns>
    /// <response code="200">Aplicação está saudável</response>
    /// <response code="503">Aplicação está com problemas</response>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), 200)]
    [ProducesResponseType(typeof(HealthErrorResponse), 503)]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Verificar conexão com o banco
            await _context.Database.CanConnectAsync();

            return Ok(new HealthResponse
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Database = "Connected"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check falhou");
            return StatusCode(503, new HealthErrorResponse
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Verifica se a aplicação está pronta para receber tráfego (readiness probe)
    /// </summary>
    /// <remarks>
    /// Usado por Kubernetes/Docker para determinar se o pod está pronto
    /// </remarks>
    /// <returns>Status de prontidão</returns>
    /// <response code="200">Aplicação pronta</response>
    [HttpGet("ready")]
    [ProducesResponseType(typeof(SimpleHealthResponse), 200)]
    public IActionResult Ready()
    {
        return Ok(new SimpleHealthResponse { Status = "Ready", Timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Verifica se a aplicação está viva (liveness probe)
    /// </summary>
    /// <remarks>
    /// Usado por Kubernetes/Docker para determinar se o pod deve ser reiniciado
    /// </remarks>
    /// <returns>Status de vida</returns>
    /// <response code="200">Aplicação viva</response>
    [HttpGet("live")]
    [ProducesResponseType(typeof(SimpleHealthResponse), 200)]
    public IActionResult Live()
    {
        return Ok(new SimpleHealthResponse { Status = "Live", Timestamp = DateTime.UtcNow });
    }
}

/// <summary>
/// Resposta de health check com sucesso
/// </summary>
public class HealthResponse
{
    /// <summary>Status da aplicação</summary>
    /// <example>Healthy</example>
    public string Status { get; set; } = string.Empty;

    /// <summary>Timestamp da verificação</summary>
    /// <example>2025-10-18T20:00:00Z</example>
    public DateTime Timestamp { get; set; }

    /// <summary>Versão da aplicação</summary>
    /// <example>1.0.0</example>
    public string Version { get; set; } = string.Empty;

    /// <summary>Status do banco de dados</summary>
    /// <example>Connected</example>
    public string Database { get; set; } = string.Empty;
}

/// <summary>
/// Resposta de health check com erro
/// </summary>
public class HealthErrorResponse
{
    /// <summary>Status da aplicação</summary>
    /// <example>Unhealthy</example>
    public string Status { get; set; } = string.Empty;

    /// <summary>Timestamp da verificação</summary>
    /// <example>2025-10-18T20:00:00Z</example>
    public DateTime Timestamp { get; set; }

    /// <summary>Mensagem de erro</summary>
    /// <example>Não foi possível conectar ao banco de dados</example>
    public string Error { get; set; } = string.Empty;
}

/// <summary>
/// Resposta simples de health check
/// </summary>
public class SimpleHealthResponse
{
    /// <summary>Status</summary>
    /// <example>Ready</example>
    public string Status { get; set; } = string.Empty;

    /// <summary>Timestamp</summary>
    /// <example>2025-10-18T20:00:00Z</example>
    public DateTime Timestamp { get; set; }
}
