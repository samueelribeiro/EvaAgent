using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Orquestracao;

namespace EvaAgent.Infra.Data.Configurations;

public class FilaDeadLetterConfiguration : IEntityTypeConfiguration<FilaDeadLetter>
{
    public void Configure(EntityTypeBuilder<FilaDeadLetter> builder)
    {
        builder.ToTable("fila_dead_letter");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FilaMensagemId)
            .IsRequired();

        builder.Property(f => f.TipoMensagem)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.ConteudoJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(f => f.ErroMensagem)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(f => f.StackTrace)
            .HasColumnType("text");

        builder.Property(f => f.TentativasProcessamento)
            .IsRequired();

        builder.Property(f => f.EnviadoDeadLetterEm)
            .IsRequired();

        // Índices
        builder.HasIndex(f => f.FilaMensagemId)
            .HasDatabaseName("idx_fila_dead_letter_mensagem");

        builder.HasIndex(f => f.TipoMensagem)
            .HasDatabaseName("idx_fila_dead_letter_tipo");

        builder.HasIndex(f => f.EnviadoDeadLetterEm)
            .HasDatabaseName("idx_fila_dead_letter_enviado");

        builder.HasIndex(f => f.Ativo)
            .HasDatabaseName("idx_fila_dead_letter_ativo");
    }
}
