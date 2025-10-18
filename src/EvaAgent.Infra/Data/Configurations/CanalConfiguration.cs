using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Canais;

namespace EvaAgent.Infra.Data.Configurations;

public class CanalConfiguration : IEntityTypeConfiguration<Canal>
{
    public void Configure(EntityTypeBuilder<Canal> builder)
    {
        builder.ToTable("canal");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.EspacoId)
            .IsRequired();

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Tipo)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.ConfiguracaoJson)
            .HasColumnType("jsonb");

        builder.Property(c => c.Habilitado)
            .IsRequired();

        // Índices
        builder.HasIndex(c => c.EspacoId)
            .HasDatabaseName("idx_canal_espaco");

        builder.HasIndex(c => c.Tipo)
            .HasDatabaseName("idx_canal_tipo");

        builder.HasIndex(c => c.Habilitado)
            .HasDatabaseName("idx_canal_habilitado");

        builder.HasIndex(c => c.Ativo)
            .HasDatabaseName("idx_canal_ativo");

        // Relacionamentos
        builder.HasOne(c => c.Espaco)
            .WithMany()
            .HasForeignKey(c => c.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Receptores)
            .WithOne(r => r.Canal)
            .HasForeignKey(r => r.CanalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
