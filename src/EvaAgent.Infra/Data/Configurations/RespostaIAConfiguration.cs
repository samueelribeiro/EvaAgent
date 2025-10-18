using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.IA;

namespace EvaAgent.Infra.Data.Configurations;

public class RespostaIAConfiguration : IEntityTypeConfiguration<RespostaIA>
{
    public void Configure(EntityTypeBuilder<RespostaIA> builder)
    {
        builder.ToTable("resposta_ia");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.SolicitacaoIAId)
            .IsRequired();

        builder.Property(r => r.Resposta)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(r => r.CustoEstimado)
            .HasPrecision(10, 6);

        builder.Property(r => r.RespondidoEm)
            .IsRequired();

        builder.Property(r => r.MetadadosJson)
            .HasColumnType("jsonb");

        // Índices
        builder.HasIndex(r => r.SolicitacaoIAId)
            .IsUnique()
            .HasDatabaseName("idx_resposta_ia_solicitacao");

        builder.HasIndex(r => r.RespondidoEm)
            .HasDatabaseName("idx_resposta_ia_data");

        builder.HasIndex(r => r.Ativo)
            .HasDatabaseName("idx_resposta_ia_ativo");

        // Relacionamentos
        builder.HasOne(r => r.SolicitacaoIA)
            .WithOne(s => s.Resposta)
            .HasForeignKey<RespostaIA>(r => r.SolicitacaoIAId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
