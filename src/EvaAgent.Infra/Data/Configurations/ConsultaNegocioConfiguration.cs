using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EvaAgent.Dominio.Entidades.Conectores;

namespace EvaAgent.Infra.Data.Configurations;

public class ConsultaNegocioConfiguration : IEntityTypeConfiguration<ConsultaNegocio>
{
    public void Configure(EntityTypeBuilder<ConsultaNegocio> builder)
    {
        builder.ToTable("consulta_negocio");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.SistemaId)
            .IsRequired();

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Descricao)
            .HasMaxLength(1000);

        builder.Property(c => c.QuerySql)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(c => c.ParametrosJson)
            .HasColumnType("jsonb");

        builder.Property(c => c.RequererAutenticacao)
            .IsRequired();

        // Índices
        builder.HasIndex(c => c.SistemaId)
            .HasDatabaseName("idx_consulta_negocio_sistema");

        builder.HasIndex(c => c.Nome)
            .HasDatabaseName("idx_consulta_negocio_nome");

        builder.HasIndex(c => c.Ativo)
            .HasDatabaseName("idx_consulta_negocio_ativo");

        // Relacionamentos
        builder.HasOne(c => c.Sistema)
            .WithMany(s => s.Consultas)
            .HasForeignKey(c => c.SistemaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
