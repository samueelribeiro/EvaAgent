using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;
using EvaAgent.Dominio.Entidades.LGPD;
using EvaAgent.Infra.Servicos.LGPD;
using FluentAssertions;
using Moq;
using Xunit;

namespace EvaAgent.Infra.Tests.Servicos;

public class PseudonimizadorServiceTests
{
    private readonly Mock<IRepositorioBase<RegistroPseudonimizacao>> _mockRepositorio;
    private readonly Mock<ICryptoService> _mockCrypto;
    private readonly PseudonimizadorService _service;

    public PseudonimizadorServiceTests()
    {
        _mockRepositorio = new Mock<IRepositorioBase<RegistroPseudonimizacao>>();
        _mockCrypto = new Mock<ICryptoService>();

        _mockCrypto.Setup(x => x.Criptografar(It.IsAny<string>()))
            .Returns<string>(texto => $"ENCRYPTED_{texto}");
        _mockCrypto.Setup(x => x.Descriptografar(It.IsAny<string>()))
            .Returns<string>(cifrado => cifrado.Replace("ENCRYPTED_", ""));
        _mockCrypto.Setup(x => x.GerarHash(It.IsAny<string>()))
            .Returns<string>(texto => $"HASH_{texto}");

        _service = new PseudonimizadorService(_mockRepositorio.Object, _mockCrypto.Object);
    }

    [Fact]
    public async Task DevePseudonimizarCPF()
    {
        // Arrange
        var textoOriginal = "O CPF do cliente é 123.456.789-00";
        var conversaId = Guid.NewGuid();

        // Act
        var textoPseudonimizado = await _service.PseudonimizarAsync(textoOriginal, conversaId);

        // Assert
        textoPseudonimizado.Should().NotContain("123.456.789-00");
        textoPseudonimizado.Should().Contain("{");
        textoPseudonimizado.Should().Contain("}");
        // O método retorna o texto pseudonimizado, registros são salvos internamente
    }

    [Fact]
    public async Task DevePseudonimizarEmail()
    {
        // Arrange
        var textoOriginal = "Entre em contato: jose@exemplo.com";
        var conversaId = Guid.NewGuid();

        // Act
        var textoPseudonimizado = await _service.PseudonimizarAsync(textoOriginal, conversaId);

        // Assert
        textoPseudonimizado.Should().NotContain("jose@exemplo.com");
        textoPseudonimizado.Should().Contain("{");
        // Email foi substituído por GUID
    }

    [Fact]
    public async Task DevePseudonimizarMultiplosDados()
    {
        // Arrange
        var textoOriginal = "Cliente José Silva, CPF 111.222.333-44, email jose@exemplo.com";
        var conversaId = Guid.NewGuid();

        // Act
        var textoPseudonimizado = await _service.PseudonimizarAsync(textoOriginal, conversaId);

        // Assert
        textoPseudonimizado.Should().NotContain("111.222.333-44");
        textoPseudonimizado.Should().NotContain("jose@exemplo.com");
        textoPseudonimizado.Should().Contain("{");
        // Múltiplos dados sensíveis foram substituídos
    }

    [Fact]
    public async Task NaoDevePseudonimizarTextoSemDadosSensiveis()
    {
        // Arrange
        var textoOriginal = "Olá, como posso ajudar?";
        var conversaId = Guid.NewGuid();

        // Act
        var textoPseudonimizado = await _service.PseudonimizarAsync(textoOriginal, conversaId);

        // Assert
        textoPseudonimizado.Should().Be(textoOriginal);
        // Texto sem dados sensíveis permanece inalterado
    }

    [Fact]
    public async Task DeveReverterPseudonimizacao()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var valorOriginal = "José Silva";
        var conversaId = Guid.NewGuid();

        var registro = new RegistroPseudonimizacao
        {
            Guid = guid,
            ValorCifrado = $"ENCRYPTED_{valorOriginal}",
            ValorOriginalHash = $"HASH_{valorOriginal}",
            TipoDado = "Nome",
            ConversaId = conversaId
        };

        _mockRepositorio
            .Setup(x => x.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<Func<RegistroPseudonimizacao, bool>>>(), default))
            .ReturnsAsync(new List<RegistroPseudonimizacao> { registro });

        var textoPseudonimizado = $"O cliente {{{guid}}} está aguardando";

        // Act
        var textoOriginal = await _service.ReverterPseudonimizacaoAsync(textoPseudonimizado, conversaId);

        // Assert
        textoOriginal.Should().Contain(valorOriginal);
        textoOriginal.Should().NotContain(guid.ToString());
    }
}
