using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Memoria;

namespace EvaAgent.Infra.Data.Configurations;

public class DocumentoTreinamentoConfiguration : IEntityTypeConfiguration<DocumentoTreinamento>
{
    public void Configure(EntityTypeBuilder<DocumentoTreinamento> builder)
    {
        builder.ToTable("documento_treinamento");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.GrupoTreinamentoId)
            .IsRequired();

        builder.Property(d => d.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Descricao)
            .HasMaxLength(1000);

        builder.Property(d => d.ConteudoOriginal)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(d => d.ConteudoProcessado)
            .HasColumnType("text");

        builder.Property(d => d.Embedding)
            .HasColumnType("text");

        builder.Property(d => d.MetadadosJson)
            .HasColumnType("jsonb");

        builder.Property(d => d.IngeridoEm)
            .IsRequired();

        // Índices
        builder.HasIndex(d => d.GrupoTreinamentoId)
            .HasDatabaseName("idx_documento_treinamento_grupo");

        builder.HasIndex(d => d.Nome)
            .HasDatabaseName("idx_documento_treinamento_nome");

        builder.HasIndex(d => d.IngeridoEm)
            .HasDatabaseName("idx_documento_treinamento_ingerido");

        builder.HasIndex(d => d.Ativo)
            .HasDatabaseName("idx_documento_treinamento_ativo");

        // Relacionamentos
        builder.HasOne(d => d.GrupoTreinamento)
            .WithMany(g => g.Documentos)
            .HasForeignKey(d => d.GrupoTreinamentoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
