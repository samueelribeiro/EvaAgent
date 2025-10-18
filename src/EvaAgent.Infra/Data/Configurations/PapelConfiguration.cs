using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Identidade;

namespace EvaAgent.Infra.Data.Configurations;

public class PapelConfiguration : IEntityTypeConfiguration<Papel>
{
    public void Configure(EntityTypeBuilder<Papel> builder)
    {
        builder.ToTable("papel");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Descricao)
            .HasMaxLength(500);

        builder.Property(p => p.Tipo)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Permissoes)
            .HasColumnType("jsonb");

        // Índices
        builder.HasIndex(p => p.Nome)
            .HasDatabaseName("idx_papel_nome");

        builder.HasIndex(p => p.Tipo)
            .HasDatabaseName("idx_papel_tipo");

        builder.HasIndex(p => p.Ativo)
            .HasDatabaseName("idx_papel_ativo");

        // Relacionamentos
        builder.HasMany(p => p.UsuarioEspacos)
            .WithOne(ue => ue.Papel)
            .HasForeignKey(ue => ue.PapelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
