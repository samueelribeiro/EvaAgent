using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.IA;

namespace EvaAgent.Infra.Data.Configurations;

public class SolicitacaoIAConfiguration : IEntityTypeConfiguration<SolicitacaoIA>
{
    public void Configure(EntityTypeBuilder<SolicitacaoIA> builder)
    {
        builder.ToTable("solicitacao_ia");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProvedorIAId)
            .IsRequired();

        builder.Property(s => s.Prompt)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(s => s.ContextoJson)
            .HasColumnType("jsonb");

        builder.Property(s => s.SolicitadoEm)
            .IsRequired();

        // Índices
        builder.HasIndex(s => s.ProvedorIAId)
            .HasDatabaseName("idx_solicitacao_ia_provedor");

        builder.HasIndex(s => s.ConversaId)
            .HasDatabaseName("idx_solicitacao_ia_conversa");

        builder.HasIndex(s => s.SolicitadoEm)
            .HasDatabaseName("idx_solicitacao_ia_data");

        builder.HasIndex(s => s.Ativo)
            .HasDatabaseName("idx_solicitacao_ia_ativo");

        // Relacionamentos
        builder.HasOne(s => s.ProvedorIA)
            .WithMany()
            .HasForeignKey(s => s.ProvedorIAId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Conversa)
            .WithMany()
            .HasForeignKey(s => s.ConversaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Resposta)
            .WithOne(r => r.SolicitacaoIA)
            .HasForeignKey<RespostaIA>(r => r.SolicitacaoIAId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
