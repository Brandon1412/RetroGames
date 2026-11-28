using Dapper;
using RetroGames.Model.Model;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositorios;

public class PlataformaRepository : IPlataformaRepository
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public PlataformaRepository(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

// Verifica se existem jogos vinculados a plataforma.
    public async Task<bool> PossuiJogosAsync(int idPlataforma)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM tb_jogo
                WHERE co_plataforma = @IdPlataforma
            );
            """;

        using var conexao = _connectionFactory.CreateConnection();

        return await conexao.ExecuteScalarAsync<bool>(
            sql, new { IdPlataforma = idPlataforma });
    }

    // Lista todas as plataformas cadastradas.
    public async Task<List<Plataforma>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_seq_plataforma AS Id,
                no_plataforma AS Nome,
                no_fabricante AS Fabricante,
                nu_ano_lancamento AS AnoLancamento,
                ds_categoria AS Categoria,
                ds_plataforma AS Descricao,
                st_ativo AS Ativo,
                (SELECT COUNT(*) FROM tb_jogo j WHERE j.co_plataforma = tb_plataforma.co_seq_plataforma) AS QuantidadeJogos
            FROM tb_plataforma
            ORDER BY no_plataforma;
            """;

        using var conexao = _connectionFactory.CreateConnection();

        var plataformas = await conexao.QueryAsync<Plataforma>(sql);

        return plataformas.ToList();
    }

    // Busca uma plataforma pelo ID.
    public async Task<Plataforma?> ObterPorIdAsync(int id)
    {
        const string sql = """
            SELECT
                co_seq_plataforma AS Id,
                no_plataforma AS Nome,
                no_fabricante AS Fabricante,
                nu_ano_lancamento AS AnoLancamento,
                ds_categoria AS Categoria,
                ds_plataforma AS Descricao,
                st_ativo AS Ativo,
                (SELECT COUNT(*) FROM tb_jogo j WHERE j.co_plataforma = tb_plataforma.co_seq_plataforma) AS QuantidadeJogos
            FROM tb_plataforma
            WHERE co_seq_plataforma = @Id;
            """;

        using var conexao = _connectionFactory.CreateConnection();

        return await conexao.QuerySingleOrDefaultAsync<Plataforma>(
            sql, new { Id = id });
    }

    // Insere uma nova plataforma e retorna seu ID.
    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        const string sql = """
            INSERT INTO tb_plataforma
                (
                    no_plataforma,
                    no_fabricante,
                    nu_ano_lancamento,
                    ds_categoria,
                    ds_plataforma,
                    st_ativo
                )
            VALUES
                (
                    @Nome,
                    @Fabricante,
                    @AnoLancamento,
                    @Categoria,
                    @Descricao,
                    @Ativo
                );
            """;

        using var conexao = _connectionFactory.CreateConnection();

        await conexao.OpenAsync();
        await conexao.ExecuteAsync(sql, new
        {
            plataforma.Nome,
            plataforma.Fabricante,
            plataforma.AnoLancamento,
            plataforma.Categoria,
            Descricao = plataforma.Descricao ?? string.Empty,
            plataforma.Ativo
        });

        return await conexao.ExecuteScalarAsync<int>(
            "SELECT LAST_INSERT_ID();");
    }

    // Atualiza os dados de uma plataforma.
    public async Task<bool> AlterarAsync(Plataforma plataforma)
    {
        const string sql = """
            UPDATE tb_plataforma
            SET
                no_plataforma = @Nome,
                no_fabricante = @Fabricante,
                nu_ano_lancamento = @AnoLancamento,
                ds_categoria = @Categoria,
                ds_plataforma = @Descricao,
                st_ativo = @Ativo
            WHERE co_seq_plataforma = @Id;
            """;

        using var conexao = _connectionFactory.CreateConnection();

        var linhasAfetadas = await conexao.ExecuteAsync(sql, new
        {
            plataforma.Id,
            plataforma.Nome,
            plataforma.Fabricante,
            plataforma.AnoLancamento,
            plataforma.Categoria,
            Descricao = plataforma.Descricao ?? string.Empty,
            plataforma.Ativo
        });

        return linhasAfetadas > 0;
    }

    // Exclui fisicamente uma plataforma.
    public async Task<bool> ExcluirAsync(int id)
    {
        const string sql = """
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @Id;
            """;

        using var conexao = _connectionFactory.CreateConnection();

        var linhasAfetadas = await conexao.ExecuteAsync(
            sql, new { Id = id });

        return linhasAfetadas > 0;
    }
}
