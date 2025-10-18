using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvaAgent.Infra.Data.Contexts;

namespace EvaAgent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(AppDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Verificar conexão com o banco
            await _context.Database.CanConnectAsync();

            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                database = "Connected"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check falhou");
            return StatusCode(503, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }

    [HttpGet("ready")]
    public IActionResult Ready()
    {
        return Ok(new { status = "Ready", timestamp = DateTime.UtcNow });
    }

    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new { status = "Live", timestamp = DateTime.UtcNow });
    }
}
