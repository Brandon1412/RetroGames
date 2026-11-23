
namespace RetroGames.WebApi.ApiModels.Plataformas;

public class PlataformaRequest
{
    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
}
