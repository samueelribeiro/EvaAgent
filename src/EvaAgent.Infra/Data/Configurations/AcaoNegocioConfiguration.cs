using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Conectores;

namespace EvaAgent.Infra.Data.Configurations;

public class AcaoNegocioConfiguration : IEntityTypeConfiguration<AcaoNegocio>
{
    public void Configure(EntityTypeBuilder<AcaoNegocio> builder)
    {
        builder.ToTable("acao_negocio");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.SistemaId)
            .IsRequired();

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Descricao)
            .HasMaxLength(1000);

        builder.Property(a => a.EndpointUrl)
            .HasMaxLength(500);

        builder.Property(a => a.MetodoHttp)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.ScriptSql)
            .HasColumnType("text");

        builder.Property(a => a.ParametrosJson)
            .HasColumnType("jsonb");

        builder.Property(a => a.RequererConfirmacao)
            .IsRequired();

        builder.Property(a => a.RequererAutenticacao)
            .IsRequired();

        // Índices
        builder.HasIndex(a => a.SistemaId)
            .HasDatabaseName("idx_acao_negocio_sistema");

        builder.HasIndex(a => a.Nome)
            .HasDatabaseName("idx_acao_negocio_nome");

        builder.HasIndex(a => a.Ativo)
            .HasDatabaseName("idx_acao_negocio_ativo");

        // Relacionamentos
        builder.HasOne(a => a.Sistema)
            .WithMany(s => s.Acoes)
            .HasForeignKey(a => a.SistemaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
