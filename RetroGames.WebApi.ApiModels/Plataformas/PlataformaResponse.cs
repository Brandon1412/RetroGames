namespace RetroGames.WebApi.ApiModels.Plataformas;

public class PlataformaResponse
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Fabricante { get; set; }

    public int? AnoLancamento { get; set; }

    public string? Categoria { get; set; }

    public string? Descricao { get; set; }

    public bool Ativo { get; set; }
}
