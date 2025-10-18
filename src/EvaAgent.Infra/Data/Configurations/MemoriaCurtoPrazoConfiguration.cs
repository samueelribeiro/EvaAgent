using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Memoria;

namespace EvaAgent.Infra.Data.Configurations;

public class MemoriaCurtoPrazoConfiguration : IEntityTypeConfiguration<MemoriaCurtoPrazo>
{
    public void Configure(EntityTypeBuilder<MemoriaCurtoPrazo> builder)
    {
        builder.ToTable("memoria_curto_prazo");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ConversaId)
            .IsRequired();

        builder.Property(m => m.Chave)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Valor)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(m => m.ExpiraEm)
            .IsRequired();

        // Índices
        builder.HasIndex(m => m.ConversaId)
            .HasDatabaseName("idx_memoria_curto_prazo_conversa");

        builder.HasIndex(m => new { m.ConversaId, m.Chave })
            .IsUnique()
            .HasDatabaseName("idx_memoria_curto_prazo_conversa_chave");

        builder.HasIndex(m => m.ExpiraEm)
            .HasDatabaseName("idx_memoria_curto_prazo_expira");

        builder.HasIndex(m => m.Ativo)
            .HasDatabaseName("idx_memoria_curto_prazo_ativo");

        // Relacionamentos
        builder.HasOne(m => m.Conversa)
            .WithMany()
            .HasForeignKey(m => m.ConversaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
