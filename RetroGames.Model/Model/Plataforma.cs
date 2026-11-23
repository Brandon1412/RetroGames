
namespace RetroGames.Model.Model;

/// <summary>Tabela tb_plataforma.</summary>
public class Plataforma
{
    /// <summary>Coluna co_seq_plataforma.</summary>
    public int Id { get; set; }

    /// <summary>Coluna no_plataforma.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Coluna ds_plataforma.</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Coluna st_ativo.</summary>
    public bool Ativo { get; set; } = true;
}
