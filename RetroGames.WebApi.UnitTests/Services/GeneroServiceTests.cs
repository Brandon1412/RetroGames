using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Servicos;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    [Fact]
    public async Task ListarAtivosAsync_DeveRetornarGenerosDoRepositorio()
    {
        var generos = new List<Genero>
        {
            new() { Id = 1, Nome = "Ação" },
            new() { Id = 2, Nome = "Aventura" }
        };

        var repository = new Mock<IGeneroRepository>();

        repository
            .Setup(r => r.ListarAtivosAsync())
            .ReturnsAsync(generos);

        var service = new GeneroService(repository.Object);

        var resultado = await service.ListarAtivosAsync();

        Assert.Same(generos, resultado);
        Assert.Equal(2, resultado.Count);
        repository.Verify(r => r.ListarAtivosAsync(), Times.Once);
    }
}
