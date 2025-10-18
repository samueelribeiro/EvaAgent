using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.LGPD;

namespace EvaAgent.Infra.Data.Configurations;

public class ConsentimentoLGPDConfiguration : IEntityTypeConfiguration<ConsentimentoLGPD>
{
    public void Configure(EntityTypeBuilder<ConsentimentoLGPD> builder)
    {
        builder.ToTable("consentimento_lgpd");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ReceptorId)
            .IsRequired();

        builder.Property(c => c.Finalidade)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Consentido)
            .IsRequired();

        builder.Property(c => c.ConsentidoEm)
            .IsRequired();

        builder.Property(c => c.IpOrigem)
            .HasMaxLength(45);

        // Índices
        builder.HasIndex(c => c.ReceptorId)
            .HasDatabaseName("idx_consentimento_lgpd_receptor");

        builder.HasIndex(c => new { c.ReceptorId, c.Finalidade })
            .HasDatabaseName("idx_consentimento_lgpd_receptor_finalidade");

        builder.HasIndex(c => c.Consentido)
            .HasDatabaseName("idx_consentimento_lgpd_consentido");

        builder.HasIndex(c => c.ConsentidoEm)
            .HasDatabaseName("idx_consentimento_lgpd_data");

        builder.HasIndex(c => c.Ativo)
            .HasDatabaseName("idx_consentimento_lgpd_ativo");

        // Relacionamentos
        builder.HasOne(c => c.Receptor)
            .WithMany()
            .HasForeignKey(c => c.ReceptorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
