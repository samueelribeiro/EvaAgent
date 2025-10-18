using Microsoft.EntityFrameworkCore;
using EvaAgent.Dominio.Entidades.Identidade;
using EvaAgent.Dominio.Entidades.Canais;
using EvaAgent.Dominio.Entidades.IA;
using EvaAgent.Dominio.Entidades.Agentes;
using EvaAgent.Dominio.Entidades.Conectores;
using EvaAgent.Dominio.Entidades.Conversas;
using EvaAgent.Dominio.Entidades.LGPD;
using EvaAgent.Dominio.Entidades.Memoria;
using EvaAgent.Dominio.Entidades.Orquestracao;
using EvaAgent.Dominio.Entidades.Observabilidade;

namespace EvaAgent.Infra.Data.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Identidade
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Espaco> Espacos => Set<Espaco>();
    public DbSet<Papel> Papeis => Set<Papel>();
    public DbSet<UsuarioEspaco> UsuarioEspacos => Set<UsuarioEspaco>();

    // Canais
    public DbSet<Canal> Canais => Set<Canal>();
    public DbSet<Receptor> Receptores => Set<Receptor>();

    // IA
    public DbSet<ProvedorIA> ProvedoresIA => Set<ProvedorIA>();
    public DbSet<SolicitacaoIA> SolicitacoesIA => Set<SolicitacaoIA>();
    public DbSet<RespostaIA> RespostasIA => Set<RespostaIA>();

    // Agentes
    public DbSet<Agente> Agentes => Set<Agente>();
    public DbSet<IntencaoAgente> IntencoesAgente => Set<IntencaoAgente>();

    // Conectores
    public DbSet<Sistema> Sistemas => Set<Sistema>();
    public DbSet<Conector> Conectores => Set<Conector>();
    public DbSet<ConsultaNegocio> ConsultasNegocio => Set<ConsultaNegocio>();
    public DbSet<AcaoNegocio> AcoesNegocio => Set<AcaoNegocio>();

    // Conversas
    public DbSet<Conversa> Conversas => Set<Conversa>();
    public DbSet<Mensagem> Mensagens => Set<Mensagem>();

    // LGPD
    public DbSet<RegistroPseudonimizacao> RegistrosPseudonimizacao => Set<RegistroPseudonimizacao>();
    public DbSet<ConsentimentoLGPD> ConsentimentosLGPD => Set<ConsentimentoLGPD>();

    // Memória
    public DbSet<MemoriaCurtoPrazo> MemoriasCurtoPrazo => Set<MemoriaCurtoPrazo>();
    public DbSet<MemoriaLongoPrazo> MemoriasLongoPrazo => Set<MemoriaLongoPrazo>();
    public DbSet<GrupoTreinamento> GruposTreinamento => Set<GrupoTreinamento>();
    public DbSet<DocumentoTreinamento> DocumentosTreinamento => Set<DocumentoTreinamento>();

    // Orquestração
    public DbSet<TarefaAgendada> TarefasAgendadas => Set<TarefaAgendada>();
    public DbSet<FilaMensagem> FilasMensagem => Set<FilaMensagem>();
    public DbSet<FilaDeadLetter> FilasDeadLetter => Set<FilaDeadLetter>();

    // Observabilidade
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();
    public DbSet<RegistroErro> RegistrosErro => Set<RegistroErro>();
    public DbSet<MetricaUso> MetricasUso => Set<MetricaUso>();
    public DbSet<RegistroExecucao> RegistrosExecucao => Set<RegistroExecucao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Convenções globais
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Nomes de tabelas em snake_case
            var tableName = entityType.GetTableName();
            if (tableName != null)
            {
                entityType.SetTableName(ToSnakeCase(tableName));
            }

            // Nomes de propriedades em snake_case
            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }

            // Índices para colunas Ativo
            if (entityType.FindProperty("Ativo") != null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex("Ativo")
                    .HasDatabaseName($"idx_{ToSnakeCase(tableName!)}_ativo");
            }

            // Índices para colunas CriadoEm
            if (entityType.FindProperty("CriadoEm") != null)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex("CriadoEm")
                    .HasDatabaseName($"idx_{ToSnakeCase(tableName!)}_criado_em");
            }
        }
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLowerInvariant(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
    }
}
