using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Identidade;

namespace EvaAgent.Infra.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuario");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.SenhaHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Telefone)
            .HasMaxLength(20);

        builder.Property(u => u.Avatar)
            .HasMaxLength(500);

        builder.Property(u => u.TimeZone)
            .HasMaxLength(50);

        builder.Property(u => u.Idioma)
            .HasMaxLength(10);

        // Índices
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("idx_usuario_email");

        builder.HasIndex(u => u.Ativo)
            .HasDatabaseName("idx_usuario_ativo");

        // Relacionamentos
        builder.HasMany(u => u.UsuarioEspacos)
            .WithOne(ue => ue.Usuario)
            .HasForeignKey(ue => ue.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
