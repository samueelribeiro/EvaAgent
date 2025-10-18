using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Orquestracao;

namespace EvaAgent.Infra.Data.Configurations;

public class FilaMensagemConfiguration : IEntityTypeConfiguration<FilaMensagem>
{
    public void Configure(EntityTypeBuilder<FilaMensagem> builder)
    {
        builder.ToTable("fila_mensagem");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.EspacoId)
            .IsRequired();

        builder.Property(f => f.TipoMensagem)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.ConteudoJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(f => f.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(f => f.TentativasProcessamento)
            .IsRequired();

        builder.Property(f => f.MaxTentativas)
            .IsRequired();

        builder.Property(f => f.ErroMensagem)
            .HasColumnType("text");

        // Índices
        builder.HasIndex(f => f.EspacoId)
            .HasDatabaseName("idx_fila_mensagem_espaco");

        builder.HasIndex(f => f.Status)
            .HasDatabaseName("idx_fila_mensagem_status");

        builder.HasIndex(f => f.TipoMensagem)
            .HasDatabaseName("idx_fila_mensagem_tipo");

        builder.HasIndex(f => f.ProcessadoEm)
            .HasDatabaseName("idx_fila_mensagem_processado");

        builder.HasIndex(f => f.Ativo)
            .HasDatabaseName("idx_fila_mensagem_ativo");

        // Relacionamentos
        builder.HasOne(f => f.Espaco)
            .WithMany()
            .HasForeignKey(f => f.EspacoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
