using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Identidade;

namespace EvaAgent.Infra.Data.Configurations;

public class UsuarioEspacoConfiguration : IEntityTypeConfiguration<UsuarioEspaco>
{
    public void Configure(EntityTypeBuilder<UsuarioEspaco> builder)
    {
        builder.ToTable("usuario_espaco");

        builder.HasKey(ue => ue.Id);

        builder.Property(ue => ue.UsuarioId)
            .IsRequired();

        builder.Property(ue => ue.EspacoId)
            .IsRequired();

        builder.Property(ue => ue.PapelId)
            .IsRequired();

        // Índices
        builder.HasIndex(ue => new { ue.UsuarioId, ue.EspacoId })
            .IsUnique()
            .HasDatabaseName("idx_usuario_espaco_unique");

        builder.HasIndex(ue => ue.EspacoId)
            .HasDatabaseName("idx_usuario_espaco_espaco");

        builder.HasIndex(ue => ue.PapelId)
            .HasDatabaseName("idx_usuario_espaco_papel");

        builder.HasIndex(ue => ue.Ativo)
            .HasDatabaseName("idx_usuario_espaco_ativo");

        // Relacionamentos
        builder.HasOne(ue => ue.Usuario)
            .WithMany(u => u.UsuarioEspacos)
            .HasForeignKey(ue => ue.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Espaco)
            .WithMany(e => e.UsuarioEspacos)
            .HasForeignKey(ue => ue.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Papel)
            .WithMany(p => p.UsuarioEspacos)
            .HasForeignKey(ue => ue.PapelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
