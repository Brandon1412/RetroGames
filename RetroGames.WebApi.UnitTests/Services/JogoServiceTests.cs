using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Excecoes;
using RetroGames.Services.Servicos;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _repositoryMock;
    private readonly JogoService _service;

    public JogoServiceTests()
    {
        _repositoryMock = new Mock<IJogoRepository>();
        _service = new JogoService(_repositoryMock.Object);
    }

    private static Jogo CriarJogo()
    {
        return new Jogo
        {
            Nome = "Super Mario World",
            Descricao = "Jogo de plataforma",
            AnoLancamento = 1990,
            Preco = 150.00m,
            Ativo = false,
            IdPlataforma = 1,
            IdCondicaoJogo = 1,
            IdsGeneros = new List<int> { 1, 2 }
        };
    }

    private void ConfigurarRelacionamentosValidos()
    {
        _repositoryMock
            .Setup(r => r.ExistePlataformaAtivaAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(r => r.ExisteCondicaoAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(r => r.GenerosExistemAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(true);
    }

    [Fact]
    public async Task InserirAsync_PlataformaInexistente_DeveLancarExcecaoENaoInserir()
    {
        var jogo = CriarJogo();

        _repositoryMock
            .Setup(r => r.ExistePlataformaAtivaAsync(jogo.IdPlataforma))
            .ReturnsAsync(false);

        var acao = () => _service.InserirAsync(jogo);

        await Assert.ThrowsAsync<RelacionamentoNaoEncontradoException>(acao);

        _repositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never);
    }

    [Fact]
    public async Task InserirAsync_CondicaoInexistente_DeveLancarExcecaoENaoInserir()
    {
        var jogo = CriarJogo();

        _repositoryMock
            .Setup(r => r.ExistePlataformaAtivaAsync(jogo.IdPlataforma))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(r => r.ExisteCondicaoAsync(jogo.IdCondicaoJogo))
            .ReturnsAsync(false);

        var acao = () => _service.InserirAsync(jogo);

        await Assert.ThrowsAsync<RelacionamentoNaoEncontradoException>(acao);

        _repositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never);

        _repositoryMock.Verify(
            r => r.GenerosExistemAsync(It.IsAny<List<int>>()),
            Times.Never);
    }

    [Fact]
    public async Task InserirAsync_GeneroInexistente_DeveLancarExcecaoENaoInserir()
    {
        var jogo = CriarJogo();

        _repositoryMock
            .Setup(r => r.ExistePlataformaAtivaAsync(jogo.IdPlataforma))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(r => r.ExisteCondicaoAsync(jogo.IdCondicaoJogo))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(r => r.GenerosExistemAsync(jogo.IdsGeneros))
            .ReturnsAsync(false);

        var acao = () => _service.InserirAsync(jogo);

        await Assert.ThrowsAsync<RelacionamentoNaoEncontradoException>(acao);

        _repositoryMock.Verify(
            r => r.InserirAsync(It.IsAny<Jogo>()),
            Times.Never);
    }

    [Fact]
    public async Task InserirAsync_RelacionamentosValidos_DeveRetornarIdEAtivarJogo()
    {
        var jogo = CriarJogo();

        ConfigurarRelacionamentosValidos();

        _repositoryMock
            .Setup(r => r.InserirAsync(jogo))
            .ReturnsAsync(10);

        var resultado = await _service.InserirAsync(jogo);

        Assert.Equal(10, resultado);
        Assert.True(jogo.Ativo);

        _repositoryMock.Verify(
            r => r.InserirAsync(jogo),
            Times.Once);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarJogosDoRepositorio()
    {
        var jogos = new List<Jogo>
        {
            new()
            {
                Id = 1,
                Nome = "Super Mario World"
            },
            new()
            {
                Id = 2,
                Nome = "Sonic"
            }
        };

        _repositoryMock
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(jogos);

        var resultado = await _service.ListarAsync();

        Assert.Equal(2, resultado.Count);
        Assert.Equal("Super Mario World", resultado[0].Nome);
        Assert.Equal("Sonic", resultado[1].Nome);

        _repositoryMock.Verify(
            r => r.ListarAsync(),
            Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_JogoInexistente_DeveLancarExcecao()
    {
        _repositoryMock
            .Setup(r => r.ObterPorIdAsync(999))
            .ReturnsAsync((Jogo?)null);

        var acao = () => _service.ObterPorIdAsync(999);

        await Assert.ThrowsAsync<KeyNotFoundException>(acao);
    }

    [Fact]
    public async Task ExcluirAsync_JogoInexistente_DeveLancarExcecaoENaoExcluir()
    {
        _repositoryMock
            .Setup(r => r.ObterPorIdAsync(999))
            .ReturnsAsync((Jogo?)null);

        var acao = () => _service.ExcluirAsync(999);

        await Assert.ThrowsAsync<KeyNotFoundException>(acao);

        _repositoryMock.Verify(
            r => r.ExcluirLogicamenteAsync(999),
            Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_JogoExistente_DeveExcluirLogicamente()
    {
        var jogo = new Jogo
        {
            Id = 1,
            Nome = "Super Mario World"
        };

        _repositoryMock
            .Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(jogo);

        _repositoryMock
            .Setup(r => r.ExcluirLogicamenteAsync(1))
            .Returns(Task.CompletedTask);

        await _service.ExcluirAsync(1);

        _repositoryMock.Verify(
            r => r.ObterPorIdAsync(1),
            Times.Once);

        _repositoryMock.Verify(
            r => r.ExcluirLogicamenteAsync(1),
            Times.Once);
    }
}
