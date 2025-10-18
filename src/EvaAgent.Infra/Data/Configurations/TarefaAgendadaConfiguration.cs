using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Orquestracao;

namespace EvaAgent.Infra.Data.Configurations;

public class TarefaAgendadaConfiguration : IEntityTypeConfiguration<TarefaAgendada>
{
    public void Configure(EntityTypeBuilder<TarefaAgendada> builder)
    {
        builder.ToTable("tarefa_agendada");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.EspacoId)
            .IsRequired();

        builder.Property(t => t.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.TipoTarefa)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.ParametrosJson)
            .HasColumnType("jsonb");

        builder.Property(t => t.CronExpressao)
            .HasMaxLength(100);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.Habilitada)
            .IsRequired();

        // Índices
        builder.HasIndex(t => t.EspacoId)
            .HasDatabaseName("idx_tarefa_agendada_espaco");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("idx_tarefa_agendada_status");

        builder.HasIndex(t => t.ProximaExecucao)
            .HasDatabaseName("idx_tarefa_agendada_proxima");

        builder.HasIndex(t => t.Habilitada)
            .HasDatabaseName("idx_tarefa_agendada_habilitada");

        builder.HasIndex(t => t.Ativo)
            .HasDatabaseName("idx_tarefa_agendada_ativo");

        // Relacionamentos
        builder.HasOne(t => t.Espaco)
            .WithMany()
            .HasForeignKey(t => t.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
