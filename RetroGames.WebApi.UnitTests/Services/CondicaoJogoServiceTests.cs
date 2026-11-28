using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Servicos;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task ListarAsync_DeveRetornarCondicoesDoRepositorio()
    {
        var condicoes = new List<CondicaoJogo>
        {
            new() { Id = 1, Descricao = "Novo" },
            new() { Id = 2, Descricao = "Usado" }
        };

        var repository = new Mock<ICondicaoJogoRepository>();

        repository
            .Setup(r => r.ListarAsync())
            .ReturnsAsync(condicoes);

        var service = new CondicaoJogoService(repository.Object);

        var resultado = await service.ListarAsync();

        Assert.Same(condicoes, resultado);
        Assert.Equal(2, resultado.Count);
        repository.Verify(r => r.ListarAsync(), Times.Once);
    }
}
