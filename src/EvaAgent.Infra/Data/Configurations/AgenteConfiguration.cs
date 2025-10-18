using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Agentes;

namespace EvaAgent.Infra.Data.Configurations;

public class AgenteConfiguration : IEntityTypeConfiguration<Agente>
{
    public void Configure(EntityTypeBuilder<Agente> builder)
    {
        builder.ToTable("agente");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.EspacoId)
            .IsRequired();

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Descricao)
            .HasMaxLength(1000);

        builder.Property(a => a.Tipo)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.PromptSistema)
            .HasColumnType("text");

        builder.Property(a => a.PalavrasChaveJson)
            .HasColumnType("jsonb");

        builder.Property(a => a.Habilitado)
            .IsRequired();

        builder.Property(a => a.Prioridade)
            .IsRequired();

        // Índices
        builder.HasIndex(a => a.EspacoId)
            .HasDatabaseName("idx_agente_espaco");

        builder.HasIndex(a => a.Tipo)
            .HasDatabaseName("idx_agente_tipo");

        builder.HasIndex(a => a.Habilitado)
            .HasDatabaseName("idx_agente_habilitado");

        builder.HasIndex(a => a.Prioridade)
            .HasDatabaseName("idx_agente_prioridade");

        builder.HasIndex(a => a.Ativo)
            .HasDatabaseName("idx_agente_ativo");

        // Relacionamentos
        builder.HasOne(a => a.Espaco)
            .WithMany()
            .HasForeignKey(a => a.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Intencoes)
            .WithOne(i => i.Agente)
            .HasForeignKey(i => i.AgenteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
