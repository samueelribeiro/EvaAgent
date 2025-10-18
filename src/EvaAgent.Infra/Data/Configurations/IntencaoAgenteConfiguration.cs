using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Agentes;

namespace EvaAgent.Infra.Data.Configurations;

public class IntencaoAgenteConfiguration : IEntityTypeConfiguration<IntencaoAgente>
{
    public void Configure(EntityTypeBuilder<IntencaoAgente> builder)
    {
        builder.ToTable("intencao_agente");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.AgenteId)
            .IsRequired();

        builder.Property(i => i.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Descricao)
            .HasMaxLength(1000);

        builder.Property(i => i.PalavrasChaveJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(i => i.ExemplosJson)
            .HasColumnType("jsonb");

        builder.Property(i => i.Confianca)
            .IsRequired()
            .HasPrecision(3, 2);

        // Índices
        builder.HasIndex(i => i.AgenteId)
            .HasDatabaseName("idx_intencao_agente");

        builder.HasIndex(i => i.Confianca)
            .HasDatabaseName("idx_intencao_confianca");

        builder.HasIndex(i => i.Ativo)
            .HasDatabaseName("idx_intencao_ativo");

        // Relacionamentos
        builder.HasOne(i => i.Agente)
            .WithMany(a => a.Intencoes)
            .HasForeignKey(i => i.AgenteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
