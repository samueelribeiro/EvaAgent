using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Observabilidade;

namespace EvaAgent.Infra.Data.Configurations;

public class RegistroAuditoriaConfiguration : IEntityTypeConfiguration<RegistroAuditoria>
{
    public void Configure(EntityTypeBuilder<RegistroAuditoria> builder)
    {
        builder.ToTable("registro_auditoria");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Entidade)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Acao)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.ValoresAntigos)
            .HasColumnType("jsonb");

        builder.Property(r => r.ValoresNovos)
            .HasColumnType("jsonb");

        builder.Property(r => r.IpOrigem)
            .HasMaxLength(45);

        builder.Property(r => r.UserAgent)
            .HasMaxLength(500);

        builder.Property(r => r.ExecutadoEm)
            .IsRequired();

        // Índices
        builder.HasIndex(r => r.UsuarioId)
            .HasDatabaseName("idx_registro_auditoria_usuario");

        builder.HasIndex(r => r.EspacoId)
            .HasDatabaseName("idx_registro_auditoria_espaco");

        builder.HasIndex(r => r.Entidade)
            .HasDatabaseName("idx_registro_auditoria_entidade");

        builder.HasIndex(r => r.EntidadeId)
            .HasDatabaseName("idx_registro_auditoria_entidade_id");

        builder.HasIndex(r => r.Acao)
            .HasDatabaseName("idx_registro_auditoria_acao");

        builder.HasIndex(r => r.ExecutadoEm)
            .HasDatabaseName("idx_registro_auditoria_executado");

        builder.HasIndex(r => r.Ativo)
            .HasDatabaseName("idx_registro_auditoria_ativo");

        // Relacionamentos
        builder.HasOne(r => r.Usuario)
            .WithMany()
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Espaco)
            .WithMany()
            .HasForeignKey(r => r.EspacoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
