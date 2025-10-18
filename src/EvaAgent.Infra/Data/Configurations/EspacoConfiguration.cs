using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Identidade;

namespace EvaAgent.Infra.Data.Configurations;

public class EspacoConfiguration : IEntityTypeConfiguration<Espaco>
{
    public void Configure(EntityTypeBuilder<Espaco> builder)
    {
        builder.ToTable("espaco");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Descricao)
            .HasMaxLength(1000);

        builder.Property(e => e.Slug)
            .HasMaxLength(100);

        builder.Property(e => e.LogoUrl)
            .HasMaxLength(500);

        // Índices
        builder.HasIndex(e => e.Slug)
            .IsUnique()
            .HasDatabaseName("idx_espaco_slug");

        builder.HasIndex(e => e.EspacoPaiId)
            .HasDatabaseName("idx_espaco_pai");

        builder.HasIndex(e => e.Ativo)
            .HasDatabaseName("idx_espaco_ativo");

        // Relacionamentos
        builder.HasOne(e => e.EspacoPai)
            .WithMany(e => e.SubEspacos)
            .HasForeignKey(e => e.EspacoPaiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.SubEspacos)
            .WithOne(e => e.EspacoPai)
            .HasForeignKey(e => e.EspacoPaiId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.UsuarioEspacos)
            .WithOne(ue => ue.Espaco)
            .HasForeignKey(ue => ue.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
