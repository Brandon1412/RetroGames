
using Dapper;
using RetroGames.Model.Model;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositorios;

public class JogoRepository : IJogoRepository
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public JogoRepository(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private const string SqlBase = """
        SELECT
            co_seq_jogo AS Id,
            no_jogo AS Nome,
            ds_jogo AS Descricao,
            nu_ano_lancamento AS AnoLancamento,
            vl_preco AS Preco,
            qt_estoque AS Estoque,
            st_ativo AS Ativo,
            dt_cadastro AS DataCadastro,
            co_plataforma AS IdPlataforma,
            co_condicao_jogo AS IdCondicaoJogo
        FROM tb_jogo
        """;

    public async Task<List<Jogo>> ListarAsync()
    {
        using var conexao = _connectionFactory.CreateConnection();

        var jogos = (await conexao.QueryAsync<Jogo>(
            SqlBase + " ORDER BY no_jogo;")).ToList();

        await CarregarGenerosAsync(conexao, jogos);

        return jogos;
    }

    public async Task<Jogo?> ObterPorIdAsync(int id)
    {
        using var conexao = _connectionFactory.CreateConnection();

        var jogo = await conexao.QuerySingleOrDefaultAsync<Jogo>(
            SqlBase + " WHERE co_seq_jogo = @Id;",
            new { Id = id });

        if (jogo != null)
        {
            await CarregarGenerosAsync(conexao, new List<Jogo> { jogo });
        }

        return jogo;
    }

    public async Task<bool> ExistePlataformaAtivaAsync(int id)
    {
        using var conexao = _connectionFactory.CreateConnection();

        return await conexao.ExecuteScalarAsync<bool>(
            """
            SELECT EXISTS(
                SELECT 1 FROM tb_plataforma
                WHERE co_seq_plataforma = @Id AND st_ativo = TRUE
            );
            """, new { Id = id });
    }

    public async Task<bool> ExisteCondicaoAsync(int id)
    {
        using var conexao = _connectionFactory.CreateConnection();

        return await conexao.ExecuteScalarAsync<bool>(
            """
            SELECT EXISTS(
                SELECT 1 FROM tb_condicao_jogo
                WHERE co_condicao_jogo = @Id
            );
            """, new { Id = id });
    }

    public async Task<bool> GenerosExistemAsync(List<int> ids)
    {
        if (ids.Count == 0) return false;

        using var conexao = _connectionFactory.CreateConnection();

        var quantidade = await conexao.ExecuteScalarAsync<int>(
            """
            SELECT COUNT(*)
            FROM tb_genero
            WHERE co_seq_genero IN @Ids;
            """, new { Ids = ids });

        return quantidade == ids.Count;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        using var conexao = _connectionFactory.CreateConnection();
        await conexao.OpenAsync();

        using var transacao = await conexao.BeginTransactionAsync();

        try
        {
            var id = await conexao.ExecuteScalarAsync<int>(
                """
                INSERT INTO tb_jogo
                (
                    no_jogo,
                    ds_jogo,
                    nu_ano_lancamento,
                    vl_preco,
                    qt_estoque,
                    st_ativo,
                    co_plataforma,
                    co_condicao_jogo
                )
                VALUES
                (
                    @Nome,
                    @Descricao,
                    @AnoLancamento,
                    @Preco,
                    @Estoque,
                    @Ativo,
                    @IdPlataforma,
                    @IdCondicaoJogo
                );

                SELECT LAST_INSERT_ID();
                """,
                jogo,
                transacao);

            await SalvarGenerosAsync(
                conexao, transacao, id, jogo.IdsGeneros);

            await transacao.CommitAsync();

            return id;
        }
        catch
        {
            await transacao.RollbackAsync();
            throw;
        }
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        using var conexao = _connectionFactory.CreateConnection();
        await conexao.OpenAsync();

        using var transacao = await conexao.BeginTransactionAsync();

        try
        {
            await conexao.ExecuteAsync(
                """
                UPDATE tb_jogo
                SET
                    no_jogo = @Nome,
                    ds_jogo = @Descricao,
                    nu_ano_lancamento = @AnoLancamento,
                    vl_preco = @Preco,
                    qt_estoque = @Estoque,
                    st_ativo = @Ativo,
                    co_plataforma = @IdPlataforma,
                    co_condicao_jogo = @IdCondicaoJogo
                WHERE co_seq_jogo = @Id;
                """,
                jogo,
                transacao);

            await conexao.ExecuteAsync(
                "DELETE FROM tb_jogo_genero WHERE co_jogo = @Id;",
                new { jogo.Id },
                transacao);

            await SalvarGenerosAsync(
                conexao, transacao, jogo.Id, jogo.IdsGeneros);

            await transacao.CommitAsync();
        }
        catch
        {
            await transacao.RollbackAsync();
            throw;
        }
    }

    public async Task ExcluirLogicamenteAsync(int id)
    {
        using var conexao = _connectionFactory.CreateConnection();

        await conexao.ExecuteAsync(
            """
            UPDATE tb_jogo
            SET st_ativo = FALSE
            WHERE co_seq_jogo = @Id;
            """,
            new { Id = id });
    }

    private static async Task SalvarGenerosAsync(
        System.Data.IDbConnection conexao,
        System.Data.IDbTransaction transacao,
        int idJogo,
        List<int> idsGeneros)
    {
        foreach (var idGenero in idsGeneros.Distinct())
        {
            await conexao.ExecuteAsync(
                """
                INSERT INTO tb_jogo_genero (co_jogo, co_genero)
                VALUES (@IdJogo, @IdGenero);
                """,
                new { IdJogo = idJogo, IdGenero = idGenero },
                transacao);
        }
    }

    private static async Task CarregarGenerosAsync(
        System.Data.IDbConnection conexao,
        List<Jogo> jogos)
    {
        if (jogos.Count == 0) return;

        var ids = jogos.Select(j => j.Id).ToArray();

        var relacionamentos = await conexao.QueryAsync<JogoGenero>(
            """
            SELECT co_jogo AS IdJogo, co_genero AS IdGenero
            FROM tb_jogo_genero
            WHERE co_jogo IN @Ids;
            """,
            new { Ids = ids });

        var agrupados = relacionamentos
            .GroupBy(r => r.IdJogo)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.Select(r => r.IdGenero).ToList());

        foreach (var jogo in jogos)
        {
            if (agrupados.TryGetValue(jogo.Id, out var generos))
                jogo.IdsGeneros = generos;
            else
                jogo.IdsGeneros = new List<int>();
        }
    }
}
