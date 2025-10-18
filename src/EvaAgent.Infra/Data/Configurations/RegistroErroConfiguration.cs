using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Observabilidade;

namespace EvaAgent.Infra.Data.Configurations;

public class RegistroErroConfiguration : IEntityTypeConfiguration<RegistroErro>
{
    public void Configure(EntityTypeBuilder<RegistroErro> builder)
    {
        builder.ToTable("registro_erro");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.CodigoCorrelacao)
            .HasMaxLength(100);

        builder.Property(r => r.TipoErro)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Mensagem)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(r => r.StackTrace)
            .HasColumnType("text");

        builder.Property(r => r.ContextoJson)
            .HasColumnType("jsonb");

        builder.Property(r => r.Severidade)
            .IsRequired();

        builder.Property(r => r.OcorridoEm)
            .IsRequired();

        builder.Property(r => r.Resolvido)
            .IsRequired();

        // Índices
        builder.HasIndex(r => r.EspacoId)
            .HasDatabaseName("idx_registro_erro_espaco");

        builder.HasIndex(r => r.CodigoCorrelacao)
            .HasDatabaseName("idx_registro_erro_correlacao");

        builder.HasIndex(r => r.TipoErro)
            .HasDatabaseName("idx_registro_erro_tipo");

        builder.HasIndex(r => r.Severidade)
            .HasDatabaseName("idx_registro_erro_severidade");

        builder.HasIndex(r => r.Resolvido)
            .HasDatabaseName("idx_registro_erro_resolvido");

        builder.HasIndex(r => r.OcorridoEm)
            .HasDatabaseName("idx_registro_erro_ocorrido");

        builder.HasIndex(r => r.Ativo)
            .HasDatabaseName("idx_registro_erro_ativo");

        // Relacionamentos
        builder.HasOne(r => r.Espaco)
            .WithMany()
            .HasForeignKey(r => r.EspacoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
