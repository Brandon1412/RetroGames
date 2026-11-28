using Dapper;
using RetroGames.Model.Model;
using RetroGames.Repository.Conexao;

namespace RetroGames.Repository.Repositorios;

public class GeneroRepository
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public GeneroRepository(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Genero>> ListarAtivosAsync()
    {
        const string sql = """
            SELECT
                co_seq_genero AS Id,
                no_genero AS Nome
            FROM tb_genero
            ORDER BY no_genero;
            """;

        using var conexao = _connectionFactory.CreateConnection();
        var generos = await conexao.QueryAsync<Genero>(sql);

        return generos.ToList();
    }
}
