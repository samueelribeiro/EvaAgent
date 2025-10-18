using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Canais;

namespace EvaAgent.Infra.Data.Configurations;

public class ReceptorConfiguration : IEntityTypeConfiguration<Receptor>
{
    public void Configure(EntityTypeBuilder<Receptor> builder)
    {
        builder.ToTable("receptor");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.CanalId)
            .IsRequired();

        builder.Property(r => r.Identificador)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(r => r.Nome)
            .HasMaxLength(200);

        builder.Property(r => r.Email)
            .HasMaxLength(255);

        builder.Property(r => r.Avatar)
            .HasMaxLength(500);

        builder.Property(r => r.MetadadosJson)
            .HasColumnType("jsonb");

        builder.Property(r => r.TomAtendimento)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.FormatoNome)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.UsarSaudacao)
            .IsRequired();

        builder.Property(r => r.Idioma)
            .HasMaxLength(10);

        builder.Property(r => r.TimeZone)
            .HasMaxLength(50);

        // Índices
        builder.HasIndex(r => r.CanalId)
            .HasDatabaseName("idx_receptor_canal");

        builder.HasIndex(r => new { r.CanalId, r.Identificador })
            .IsUnique()
            .HasDatabaseName("idx_receptor_canal_identificador");

        builder.HasIndex(r => r.Email)
            .HasDatabaseName("idx_receptor_email");

        builder.HasIndex(r => r.Ativo)
            .HasDatabaseName("idx_receptor_ativo");

        // Relacionamentos
        builder.HasOne(r => r.Canal)
            .WithMany(c => c.Receptores)
            .HasForeignKey(r => r.CanalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Conversas)
            .WithOne(c => c.Receptor)
            .HasForeignKey(c => c.ReceptorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
