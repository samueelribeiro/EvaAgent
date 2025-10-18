using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.LGPD;

namespace EvaAgent.Infra.Data.Configurations;

public class RegistroPseudonimizacaoConfiguration : IEntityTypeConfiguration<RegistroPseudonimizacao>
{
    public void Configure(EntityTypeBuilder<RegistroPseudonimizacao> builder)
    {
        builder.ToTable("registro_pseudonimizacao");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Guid)
            .IsRequired();

        builder.Property(r => r.ValorOriginalHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(r => r.ValorCifrado)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.TipoDado)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.PseudonimizadoEm)
            .IsRequired();

        // Índices
        builder.HasIndex(r => r.Guid)
            .IsUnique()
            .HasDatabaseName("idx_registro_pseudonimizacao_guid");

        builder.HasIndex(r => r.ValorOriginalHash)
            .HasDatabaseName("idx_registro_pseudonimizacao_hash");

        builder.HasIndex(r => r.ConversaId)
            .HasDatabaseName("idx_registro_pseudonimizacao_conversa");

        builder.HasIndex(r => r.SolicitacaoIAId)
            .HasDatabaseName("idx_registro_pseudonimizacao_solicitacao");

        builder.HasIndex(r => r.ExpiraEm)
            .HasDatabaseName("idx_registro_pseudonimizacao_expira");

        builder.HasIndex(r => r.Ativo)
            .HasDatabaseName("idx_registro_pseudonimizacao_ativo");

        // Relacionamentos
        builder.HasOne(r => r.Conversa)
            .WithMany()
            .HasForeignKey(r => r.ConversaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.SolicitacaoIA)
            .WithMany()
            .HasForeignKey(r => r.SolicitacaoIAId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
