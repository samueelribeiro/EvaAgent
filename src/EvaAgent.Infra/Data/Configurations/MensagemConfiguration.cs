using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Conversas;

namespace EvaAgent.Infra.Data.Configurations;

public class MensagemConfiguration : IEntityTypeConfiguration<Mensagem>
{
    public void Configure(EntityTypeBuilder<Mensagem> builder)
    {
        builder.ToTable("mensagem");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ConversaId)
            .IsRequired();

        builder.Property(m => m.Direcao)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.Conteudo)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.EnviadaEm)
            .IsRequired();

        builder.Property(m => m.MidiaUrl)
            .HasMaxLength(500);

        builder.Property(m => m.TipoMidia)
            .HasMaxLength(50);

        builder.Property(m => m.MetadadosJson)
            .HasColumnType("jsonb");

        builder.Property(m => m.IdExterno)
            .HasMaxLength(255);

        // Índices
        builder.HasIndex(m => m.ConversaId)
            .HasDatabaseName("idx_mensagem_conversa");

        builder.HasIndex(m => m.Status)
            .HasDatabaseName("idx_mensagem_status");

        builder.HasIndex(m => m.EnviadaEm)
            .HasDatabaseName("idx_mensagem_enviada");

        builder.HasIndex(m => m.IdExterno)
            .HasDatabaseName("idx_mensagem_externo");

        builder.HasIndex(m => m.Ativo)
            .HasDatabaseName("idx_mensagem_ativo");

        // Relacionamentos
        builder.HasOne(m => m.Conversa)
            .WithMany(c => c.Mensagens)
            .HasForeignKey(m => m.ConversaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
