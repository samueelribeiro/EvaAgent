using EvaAgent.Dominio.Entidades.Identidade;
using FluentAssertions;
using Xunit;

namespace EvaAgent.Dominio.Tests.Entidades;

public class UsuarioTests
{
    [Fact]
    public void DeveCriarUsuarioComPropriedadesCorretas()
    {
        // Arrange & Act
        var usuario = new Usuario
        {
            Nome = "João Silva",
            Email = "joao@exemplo.com",
            SenhaHash = "hash123",
            Telefone = "11999999999",
            Idioma = "pt-BR"
        };

        // Assert
        usuario.Nome.Should().Be("João Silva");
        usuario.Email.Should().Be("joao@exemplo.com");
        usuario.SenhaHash.Should().Be("hash123");
        usuario.Telefone.Should().Be("11999999999");
        usuario.EmailVerificado.Should().BeFalse();
        usuario.Ativo.Should().BeTrue();
        usuario.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DevePermitirVerificarEmail()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nome = "Maria",
            Email = "maria@exemplo.com",
            SenhaHash = "hash"
        };

        // Act
        usuario.EmailVerificado = true;

        // Assert
        usuario.EmailVerificado.Should().BeTrue();
    }

    [Fact]
    public void DeveRegistrarUltimoAcesso()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nome = "Pedro",
            Email = "pedro@exemplo.com",
            SenhaHash = "hash"
        };
        var dataAcesso = DateTime.UtcNow;

        // Act
        usuario.UltimoAcesso = dataAcesso;

        // Assert
        usuario.UltimoAcesso.Should().Be(dataAcesso);
    }
}
