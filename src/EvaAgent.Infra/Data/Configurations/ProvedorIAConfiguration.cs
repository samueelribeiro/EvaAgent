using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.IA;

namespace EvaAgent.Infra.Data.Configurations;

public class ProvedorIAConfiguration : IEntityTypeConfiguration<ProvedorIA>
{
    public void Configure(EntityTypeBuilder<ProvedorIA> builder)
    {
        builder.ToTable("provedor_ia");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.EspacoId)
            .IsRequired();

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Tipo)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.UrlBase)
            .HasMaxLength(500);

        builder.Property(p => p.ChaveApi)
            .HasMaxLength(500);

        builder.Property(p => p.Modelo)
            .HasMaxLength(100);

        builder.Property(p => p.Temperatura)
            .HasPrecision(3, 2);

        builder.Property(p => p.Habilitado)
            .IsRequired();

        // Índices
        builder.HasIndex(p => p.EspacoId)
            .HasDatabaseName("idx_provedor_ia_espaco");

        builder.HasIndex(p => p.Tipo)
            .HasDatabaseName("idx_provedor_ia_tipo");

        builder.HasIndex(p => p.Habilitado)
            .HasDatabaseName("idx_provedor_ia_habilitado");

        builder.HasIndex(p => p.Ativo)
            .HasDatabaseName("idx_provedor_ia_ativo");

        // Relacionamentos
        builder.HasOne(p => p.Espaco)
            .WithMany()
            .HasForeignKey(p => p.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
