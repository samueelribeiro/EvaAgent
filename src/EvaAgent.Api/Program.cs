using Microsoft.EntityFrameworkCore;
using EvaAgent.Infra.Data.Contexts;
using EvaAgent.Infra.Data.Repositories;
using EvaAgent.Infra.Servicos.LGPD;
using EvaAgent.Infra.Servicos.Agentes;
using EvaAgent.Aplicacao.Services;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Configuração de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// Configure Entity Framework Core with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsAssembly("AgenteIA.Infra")
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
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
