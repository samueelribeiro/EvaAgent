using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Memoria;

namespace EvaAgent.Infra.Data.Configurations;

public class GrupoTreinamentoConfiguration : IEntityTypeConfiguration<GrupoTreinamento>
{
    public void Configure(EntityTypeBuilder<GrupoTreinamento> builder)
    {
        builder.ToTable("grupo_treinamento");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.EspacoId)
            .IsRequired();

        builder.Property(g => g.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Descricao)
            .HasMaxLength(1000);

        builder.Property(g => g.Tags)
            .HasMaxLength(500);

        // Índices
        builder.HasIndex(g => g.EspacoId)
            .HasDatabaseName("idx_grupo_treinamento_espaco");

        builder.HasIndex(g => g.Nome)
            .HasDatabaseName("idx_grupo_treinamento_nome");

        builder.HasIndex(g => g.Ativo)
            .HasDatabaseName("idx_grupo_treinamento_ativo");

        // Relacionamentos
        builder.HasOne(g => g.Espaco)
            .WithMany()
            .HasForeignKey(g => g.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Documentos)
            .WithOne(d => d.GrupoTreinamento)
            .HasForeignKey(d => d.GrupoTreinamentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
