using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Conectores;

namespace EvaAgent.Infra.Data.Configurations;

public class SistemaConfiguration : IEntityTypeConfiguration<Sistema>
{
    public void Configure(EntityTypeBuilder<Sistema> builder)
    {
        builder.ToTable("sistema");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.EspacoId)
            .IsRequired();

        builder.Property(s => s.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Descricao)
            .HasMaxLength(1000);

        builder.Property(s => s.VersaoApi)
            .HasMaxLength(50);

        // Índices
        builder.HasIndex(s => s.EspacoId)
            .HasDatabaseName("idx_sistema_espaco");

        builder.HasIndex(s => s.Nome)
            .HasDatabaseName("idx_sistema_nome");

        builder.HasIndex(s => s.Ativo)
            .HasDatabaseName("idx_sistema_ativo");

        // Relacionamentos
        builder.HasOne(s => s.Espaco)
            .WithMany()
            .HasForeignKey(s => s.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Conectores)
            .WithOne(c => c.Sistema)
            .HasForeignKey(c => c.SistemaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Consultas)
            .WithOne(c => c.Sistema)
            .HasForeignKey(c => c.SistemaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Acoes)
            .WithOne(a => a.Sistema)
            .HasForeignKey(a => a.SistemaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
