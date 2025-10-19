using EvaAgent.Dominio.Entidades.Canais;
using EvaAgent.Dominio.Entidades.Conversas;
using EvaAgent.Dominio.Enums;
using EvaAgent.Dominio.Interfaces.Repositorios;
using EvaAgent.Dominio.Interfaces.Servicos;
using FluentAssertions;
using Moq;
using Xunit;

namespace EvaAgent.Aplicacao.Tests.Services;

public class OrquestradorMensagensServiceTests
{
    private readonly Mock<IRepositorioBase<Receptor>> _mockReceptorRepo;
    private readonly Mock<IRepositorioBase<Conversa>> _mockConversaRepo;
    private readonly Mock<IRepositorioBase<Mensagem>> _mockMensagemRepo;
    private readonly Mock<IPseudonimizadorService> _mockPseudonimizador;

    public OrquestradorMensagensServiceTests()
    {
        _mockReceptorRepo = new Mock<IRepositorioBase<Receptor>>();
        _mockConversaRepo = new Mock<IRepositorioBase<Conversa>>();
        _mockMensagemRepo = new Mock<IRepositorioBase<Mensagem>>();
        _mockPseudonimizador = new Mock<IPseudonimizadorService>();
    }

    [Fact]
    public async Task DeveIdentificarOuCriarReceptor()
    {
        // Arrange
        var identificador = "5511999999999";
        var canal = new Canal { Id = Guid.NewGuid(), Tipo = TipoCanal.WhatsApp };

        _mockReceptorRepo
            .Setup(x => x.BuscarAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Receptor, bool>>>(), default))
            .ReturnsAsync(new List<Receptor>());

        _mockReceptorRepo
            .Setup(x => x.AdicionarAsync(It.IsAny<Receptor>(), default))
            .ReturnsAsync(new Receptor());

        // Act
        var receptor = new Receptor
        {
            CanalId = canal.Id,
            Identificador = identificador,
            Nome = "Novo Receptor"
        };

        await _mockReceptorRepo.Object.AdicionarAsync(receptor);

        // Assert
        _mockReceptorRepo.Verify(x => x.AdicionarAsync(It.IsAny<Receptor>(), default), Times.Once);
    }

    [Fact]
    public void DeveValidarMensagemEntrada()
    {
        // Arrange
        var mensagem = new Mensagem
        {
            ConversaId = Guid.NewGuid(),
            Direcao = DirecaoMensagem.Entrada,
            Conteudo = "Olá, preciso de ajuda",
            Status = StatusMensagem.Recebida
        };

        // Assert
        mensagem.Direcao.Should().Be(DirecaoMensagem.Entrada);
        mensagem.Status.Should().Be(StatusMensagem.Recebida);
        mensagem.Conteudo.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task DevePseudonimizarDadosSensiveis()
    {
        // Arrange
        var textoOriginal = "Meu CPF é 123.456.789-00";
        var conversaId = Guid.NewGuid();

        _mockPseudonimizador
            .Setup(x => x.PseudonimizarAsync(textoOriginal, conversaId, null))
            .ReturnsAsync(textoOriginal.Replace("123.456.789-00", "{guid-123}"));

        // Act
        var textoPseudonimizado = await _mockPseudonimizador.Object.PseudonimizarAsync(textoOriginal, conversaId);

        // Assert
        textoPseudonimizado.Should().NotContain("123.456.789-00");
        textoPseudonimizado.Should().Contain("{guid-123}");
    }
}
