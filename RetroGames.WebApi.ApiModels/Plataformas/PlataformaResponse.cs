
namespace RetroGames.WebApi.ApiModels.Plataformas;

public class PlataformaResponse
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public bool Ativo { get; set; }
}
