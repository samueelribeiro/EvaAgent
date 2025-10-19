using EvaAgent.Dominio.Interfaces.Servicos;
using EvaAgent.Infra.Servicos.LGPD;
using FluentAssertions;
using Xunit;

namespace EvaAgent.Dominio.Tests.Servicos;

public class CryptoServiceTests
{
    private readonly ICryptoService _cryptoService;

    public CryptoServiceTests()
    {
        // Chaves de teste (não usar em produção!)
        var key = "Change-This-Key-To-A-32-Character-String!!"; // 32 chars
        var iv = "Change-This-IV!!"; // 16 chars
        _cryptoService = new CryptoService(key, iv);
    }

    [Fact]
    public void DeveCriptografarEDescriptografarTexto()
    {
        // Arrange
        var textoOriginal = "José Silva";

        // Act
        var textoCifrado = _cryptoService.Criptografar(textoOriginal);
        var textoDecifrado = _cryptoService.Descriptografar(textoCifrado);

        // Assert
        textoCifrado.Should().NotBe(textoOriginal);
        textoDecifrado.Should().Be(textoOriginal);
    }

    [Fact]
    public void DeveGerarHashConsistente()
    {
        // Arrange
        var texto = "123.456.789-00";

        // Act
        var hash1 = _cryptoService.GerarHash(texto);
        var hash2 = _cryptoService.GerarHash(texto);

        // Assert
        hash1.Should().Be(hash2);
        hash1.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void DeveVerificarHashCorretamente()
    {
        // Arrange
        var texto = "teste@exemplo.com";
        var hash = _cryptoService.GerarHash(texto);

        // Act & Assert
        _cryptoService.VerificarHash(texto, hash).Should().BeTrue();
        _cryptoService.VerificarHash("outro@exemplo.com", hash).Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NaoDeveCriptografarTextoVazio(string textoVazio)
    {
        // Act
        Action act = () => _cryptoService.Criptografar(textoVazio);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DeveCriptografarDiferentementeTextosDiferentes()
    {
        // Arrange
        var texto1 = "João Silva";
        var texto2 = "Maria Santos";

        // Act
        var cifrado1 = _cryptoService.Criptografar(texto1);
        var cifrado2 = _cryptoService.Criptografar(texto2);

        // Assert
        cifrado1.Should().NotBe(cifrado2);
    }
}
