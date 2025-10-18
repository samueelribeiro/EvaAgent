using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Observabilidade;

namespace EvaAgent.Infra.Data.Configurations;

public class MetricaUsoConfiguration : IEntityTypeConfiguration<MetricaUso>
{
    public void Configure(EntityTypeBuilder<MetricaUso> builder)
    {
        builder.ToTable("metrica_uso");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.EspacoId)
            .IsRequired();

        builder.Property(m => m.TipoMetrica)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Recurso)
            .HasMaxLength(200);

        builder.Property(m => m.Quantidade)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(m => m.UnidadeMedida)
            .HasMaxLength(50);

        builder.Property(m => m.DimensoesJson)
            .HasColumnType("jsonb");

        builder.Property(m => m.MedidoEm)
            .IsRequired();

        // Índices
        builder.HasIndex(m => m.EspacoId)
            .HasDatabaseName("idx_metrica_uso_espaco");

        builder.HasIndex(m => m.TipoMetrica)
            .HasDatabaseName("idx_metrica_uso_tipo");

        builder.HasIndex(m => m.Recurso)
            .HasDatabaseName("idx_metrica_uso_recurso");

        builder.HasIndex(m => m.MedidoEm)
            .HasDatabaseName("idx_metrica_uso_medido");

        builder.HasIndex(m => new { m.EspacoId, m.TipoMetrica, m.MedidoEm })
            .HasDatabaseName("idx_metrica_uso_espaco_tipo_data");

        builder.HasIndex(m => m.Ativo)
            .HasDatabaseName("idx_metrica_uso_ativo");

        // Relacionamentos
        builder.HasOne(m => m.Espaco)
            .WithMany()
            .HasForeignKey(m => m.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
