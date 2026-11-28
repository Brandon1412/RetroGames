
using RetroGames.WebApi.ApiModels.Plataformas;
using RetroGames.WebApi.Validadores.Plataformas;

namespace RetroGames.WebApi.UnitTests;

public class PlataformaRequestValidatorTests
{
    private readonly PlataformaRequestValidator _validator = new();

    [Theory]
    [InlineData(1971, false)]
    [InlineData(1972, true)]
    public void DeveValidarAnoMinimo(int ano, bool esperado)
    {
        var request = CriarRequest(ano);

        var resultado = _validator.Validate(request);

        Assert.Equal(
            esperado,
            resultado.IsValid);
    }

    [Fact]
    public void DeveAceitarAnoAtual()
    {
        var request = CriarRequest(DateTime.Now.Year);

        var resultado = _validator.Validate(request);

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public void DeveRejeitarAnoFuturo()
    {
        var request = CriarRequest(DateTime.Now.Year + 1);

        var resultado = _validator.Validate(request);

        Assert.False(resultado.IsValid);
        Assert.Contains(
            resultado.Errors,
            erro => erro.PropertyName == nameof(PlataformaRequest.AnoLancamento));
    }

    [Fact]
    public void DeveAceitarAnoNaoInformado()
    {
        var request = CriarRequest(null);

        var resultado = _validator.Validate(request);

        Assert.True(resultado.IsValid);
    }

    private static PlataformaRequest CriarRequest(int? ano)
    {
        return new PlataformaRequest
        {
            Nome = "Plataforma de teste",
            AnoLancamento = ano
        };
    }
}
