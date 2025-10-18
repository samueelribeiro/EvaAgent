using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Memoria;

namespace EvaAgent.Infra.Data.Configurations;

public class MemoriaLongoPrazoConfiguration : IEntityTypeConfiguration<MemoriaLongoPrazo>
{
    public void Configure(EntityTypeBuilder<MemoriaLongoPrazo> builder)
    {
        builder.ToTable("memoria_longo_prazo");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ReceptorId)
            .IsRequired();

        builder.Property(m => m.Chave)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Valor)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(m => m.Categoria)
            .HasMaxLength(100);

        // Índices
        builder.HasIndex(m => m.ReceptorId)
            .HasDatabaseName("idx_memoria_longo_prazo_receptor");

        builder.HasIndex(m => new { m.ReceptorId, m.Chave })
            .HasDatabaseName("idx_memoria_longo_prazo_receptor_chave");

        builder.HasIndex(m => m.Categoria)
            .HasDatabaseName("idx_memoria_longo_prazo_categoria");

        builder.HasIndex(m => m.ImportanciaScore)
            .HasDatabaseName("idx_memoria_longo_prazo_importancia");

        builder.HasIndex(m => m.Ativo)
            .HasDatabaseName("idx_memoria_longo_prazo_ativo");

        // Relacionamentos
        builder.HasOne(m => m.Receptor)
            .WithMany()
            .HasForeignKey(m => m.ReceptorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
