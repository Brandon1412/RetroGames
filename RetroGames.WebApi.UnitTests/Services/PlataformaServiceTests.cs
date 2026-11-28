using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Excecoes;
using RetroGames.Services.Servicos;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _repositoryMock;
    private readonly PlataformaService _service;

    public PlataformaServiceTests()
    {
        _repositoryMock = new Mock<IPlataformaRepository>();

        _service = new PlataformaService(_repositoryMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_PlataformaInexistente_DeveLancarExcecao()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.ObterPorIdAsync(999))
            .ReturnsAsync((Plataforma?)null);

        // Act
        var acao = () => _service.ObterPorIdAsync(999);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(acao);
    }

    [Fact]
    public async Task InserirAsync_NomeDuplicado_DeveLancarExcecaoENaoInserir()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Nome = "PlayStation"
        };

        _repositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Plataforma>
            {
                new() { Id = 1, Nome = "PlayStation" }
            });

        // Act
        var acao = () => _service.InserirAsync(plataforma);

        // Assert
        await Assert.ThrowsAsync<RegistroDuplicadoException>(acao);

        _repositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Plataforma>()),
            Times.Never);
    }

    [Fact]
    public async Task InserirAsync_RepositorioDetectaDuplicidade_DevePropagarExcecao()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Nome = "Dreamcast"
        };

        _repositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Plataforma>());

        _repositoryMock
            .Setup(r => r.InserirAsync(plataforma))
            .ThrowsAsync(new RegistroDuplicadoException(
                "Plataforma duplicada."));

        // Act
        var acao = () => _service.InserirAsync(plataforma);

        // Assert
        await Assert.ThrowsAsync<RegistroDuplicadoException>(acao);

        _repositoryMock.Verify(
            r => r.InserirAsync(plataforma),
            Times.Once);
    }

    [Fact]
    public async Task InserirAsync_NomeLivre_DeveRetornarIdGerado()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Nome = "Dreamcast",
            Ativo = false
        };

        _repositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Plataforma>());

        _repositoryMock
            .Setup(r => r.InserirAsync(plataforma))
            .ReturnsAsync(10);

        // Act
        var resultado = await _service.InserirAsync(plataforma);

        // Assert
        Assert.Equal(10, resultado);
        Assert.True(plataforma.Ativo);

        _repositoryMock.Verify(
            r => r.InserirAsync(plataforma),
            Times.Once);
    }

    [Fact]
    public async Task AlterarAsync_MantendoProprioNome_DeveAlterarNormalmente()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Id = 1,
            Nome = "Nintendo"
        };

        _repositoryMock
            .Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(new Plataforma
            {
                Id = 1,
                Nome = "Nintendo"
            });

        _repositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(new List<Plataforma>
            {
                new() { Id = 1, Nome = "Nintendo" },
                new() { Id = 2, Nome = "Sega" }
            });

        _repositoryMock
            .Setup(r => r.AlterarAsync(plataforma))
            .ReturnsAsync(true);

        // Act
        await _service.AlterarAsync(plataforma);

        // Assert
        _repositoryMock.Verify(
            r => r.AlterarAsync(plataforma),
            Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_PlataformaComJogos_DeveLancarExcecaoENaoExcluir()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(new Plataforma
            {
                Id = 1,
                Nome = "Nintendo"
            });

        _repositoryMock
            .Setup(r => r.PossuiJogosAsync(1))
            .ReturnsAsync(true);

        // Act
        var acao = () => _service.ExcluirAsync(1);

        // Assert
        await Assert.ThrowsAsync<PlataformaPossuiJogosException>(acao);

        _repositoryMock.Verify(
            r => r.ExcluirAsync(1),
            Times.Never);
    }
}