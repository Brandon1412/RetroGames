using Dapper;
using RetroGames.Model.Model;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositorios;

public class CondicaoJogoRepository : ICondicaoJogoRepository
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public CondicaoJogoRepository(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<CondicaoJogo>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_condicao_jogo AS Id,
                ds_condicao_jogo AS Descricao
            FROM tb_condicao_jogo
            ORDER BY co_condicao_jogo;
            """;

        using var conexao = _connectionFactory.CreateConnection();

        var condicoes = await conexao.QueryAsync<CondicaoJogo>(sql);

        return condicoes.ToList();
    }
}
