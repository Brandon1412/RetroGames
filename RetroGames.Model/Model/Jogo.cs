
namespace RetroGames.Model.Model;

/// <summary>Tabela tb_jogo.</summary>
public class Jogo
{
    /// <summary>Coluna co_seq_jogo.</summary>
    public int Id { get; set; }

    /// <summary>Coluna no_jogo.</summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>Coluna ds_jogo.</summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Coluna vl_preco.</summary>
    public decimal Preco { get; set; }

    /// <summary>Coluna qt_estoque.</summary>
    public int Estoque { get; set; }

    /// <summary>Coluna st_ativo.</summary>
    public bool Ativo { get; set; } = true;

    /// <summary>Coluna dt_cadastro.</summary>
    public DateTime DataCadastro { get; set; }

    /// <summary>Coluna co_plataforma.</summary>
    public int IdPlataforma { get; set; }

    /// <summary>Coluna co_condicao_jogo.</summary>
    public int IdCondicaoJogo { get; set; }
}
