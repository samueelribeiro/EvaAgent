using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Conectores;

namespace EvaAgent.Infra.Data.Configurations;

public class ConectorConfiguration : IEntityTypeConfiguration<Conector>
{
    public void Configure(EntityTypeBuilder<Conector> builder)
    {
        builder.ToTable("conector");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.SistemaId)
            .IsRequired();

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Tipo)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.UrlBase)
            .HasMaxLength(500);

        builder.Property(c => c.ChaveApi)
            .HasMaxLength(500);

        builder.Property(c => c.HeadersJson)
            .HasColumnType("jsonb");

        builder.Property(c => c.TipoBancoDados)
            .HasConversion<string>();

        builder.Property(c => c.StringConexao)
            .HasMaxLength(1000);

        builder.Property(c => c.Habilitado)
            .IsRequired();

        // Índices
        builder.HasIndex(c => c.SistemaId)
            .HasDatabaseName("idx_conector_sistema");

        builder.HasIndex(c => c.Tipo)
            .HasDatabaseName("idx_conector_tipo");

        builder.HasIndex(c => c.Habilitado)
            .HasDatabaseName("idx_conector_habilitado");

        builder.HasIndex(c => c.Ativo)
            .HasDatabaseName("idx_conector_ativo");

        // Relacionamentos
        builder.HasOne(c => c.Sistema)
            .WithMany(s => s.Conectores)
            .HasForeignKey(c => c.SistemaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
