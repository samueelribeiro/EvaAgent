using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Observabilidade;

namespace EvaAgent.Infra.Data.Configurations;

public class RegistroExecucaoConfiguration : IEntityTypeConfiguration<RegistroExecucao>
{
    public void Configure(EntityTypeBuilder<RegistroExecucao> builder)
    {
        builder.ToTable("registro_execucao");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.EspacoId)
            .IsRequired();

        builder.Property(r => r.TipoExecucao)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.NomeRecurso)
            .HasMaxLength(200);

        builder.Property(r => r.ParametrosJson)
            .HasColumnType("jsonb");

        builder.Property(r => r.ResultadoJson)
            .HasColumnType("jsonb");

        builder.Property(r => r.Sucesso)
            .IsRequired();

        builder.Property(r => r.TempoExecucaoMs)
            .IsRequired();

        builder.Property(r => r.ExecutadoEm)
            .IsRequired();

        // Índices
        builder.HasIndex(r => r.EspacoId)
            .HasDatabaseName("idx_registro_execucao_espaco");

        builder.HasIndex(r => r.ConversaId)
            .HasDatabaseName("idx_registro_execucao_conversa");

        builder.HasIndex(r => r.TipoExecucao)
            .HasDatabaseName("idx_registro_execucao_tipo");

        builder.HasIndex(r => r.RecursoId)
            .HasDatabaseName("idx_registro_execucao_recurso");

        builder.HasIndex(r => r.Sucesso)
            .HasDatabaseName("idx_registro_execucao_sucesso");

        builder.HasIndex(r => r.ExecutadoEm)
            .HasDatabaseName("idx_registro_execucao_executado");

        builder.HasIndex(r => new { r.EspacoId, r.TipoExecucao, r.ExecutadoEm })
            .HasDatabaseName("idx_registro_execucao_espaco_tipo_data");

        builder.HasIndex(r => r.Ativo)
            .HasDatabaseName("idx_registro_execucao_ativo");

        // Relacionamentos
        builder.HasOne(r => r.Espaco)
            .WithMany()
            .HasForeignKey(r => r.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Conversa)
            .WithMany()
            .HasForeignKey(r => r.ConversaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
