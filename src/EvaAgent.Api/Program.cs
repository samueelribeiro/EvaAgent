using EvaAgent.Aplicacao.Services;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;
using EvaAgent.Infra.Configuracoes;
using EvaAgent.Infra.Data.Contexts;
using EvaAgent.Infra.Data.Repositories;
using EvaAgent.Infra.Servicos.Agentes;
using EvaAgent.Infra.Servicos.LGPD;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure Options
builder.Services.Configure<CryptoOptions>(
    builder.Configuration.GetSection(CryptoOptions.SectionName));

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EvaAgent API",
        Version = "v1",
        Description = "API do Agente Multissistema - Plataforma Orquestradora de IA com conformidade LGPD",
        Contact = new OpenApiContact
        {
            Name = "EvaAgent",
            Email = "contato@evaagent.com"
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Health checks (sem verificação de DB por enquanto)
builder.Services.AddHealthChecks();

// Configure Entity Framework Core with PostgreSQL
// NOTA: Comentado temporariamente para permitir que a API suba sem banco de dados
// Descomente quando o PostgreSQL estiver disponível
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=evaagent;Username=postgres;Password=postgres",
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("EvaAgent.Infra")
    )
);

// Register Repositories
builder.Services.AddScoped(typeof(IRepositorioBase<>), typeof(RepositorioBase<>));

// Register Infrastructure Services
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<IPseudonimizadorService, PseudonimizadorService>();
builder.Services.AddScoped<IIntentResolverService, IntentResolverService>();

// Register HttpClient
builder.Services.AddHttpClient();

// Register Application Services
builder.Services.AddScoped<OrquestradorMensagensService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();

// Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EvaAgent API v1");
    c.RoutePrefix = "swagger"; // Swagger UI em /swagger
    c.DocumentTitle = "EvaAgent API - Documentação";
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

try
{
    app.Logger.LogInformation("Iniciando EvaAgent API...");
    app.Logger.LogInformation("Swagger UI disponível em: /swagger");
    app.Logger.LogInformation("OpenAPI JSON disponível em: /swagger/v1/swagger.json");
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Erro fatal ao iniciar a aplicação");
    throw;
}

// Tornar a classe Program acessível para testes de integração
public partial class Program { }
