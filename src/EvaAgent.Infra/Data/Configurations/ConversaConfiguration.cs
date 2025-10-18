using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Conversas;

namespace EvaAgent.Infra.Data.Configurations;

public class ConversaConfiguration : IEntityTypeConfiguration<Conversa>
{
    public void Configure(EntityTypeBuilder<Conversa> builder)
    {
        builder.ToTable("conversa");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ReceptorId)
            .IsRequired();

        builder.Property(c => c.Titulo)
            .HasMaxLength(200);

        builder.Property(c => c.IniciadaEm)
            .IsRequired();

        builder.Property(c => c.ResumoJson)
            .HasColumnType("jsonb");

        builder.Property(c => c.Arquivada)
            .IsRequired();

        // Índices
        builder.HasIndex(c => c.ReceptorId)
            .HasDatabaseName("idx_conversa_receptor");

        builder.HasIndex(c => c.AgenteId)
            .HasDatabaseName("idx_conversa_agente");

        builder.HasIndex(c => c.IniciadaEm)
            .HasDatabaseName("idx_conversa_iniciada");

        builder.HasIndex(c => c.Arquivada)
            .HasDatabaseName("idx_conversa_arquivada");

        builder.HasIndex(c => c.Ativo)
            .HasDatabaseName("idx_conversa_ativo");

        // Relacionamentos
        builder.HasOne(c => c.Receptor)
            .WithMany(r => r.Conversas)
            .HasForeignKey(c => c.ReceptorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Agente)
            .WithMany()
            .HasForeignKey(c => c.AgenteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Mensagens)
            .WithOne(m => m.Conversa)
            .HasForeignKey(m => m.ConversaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
