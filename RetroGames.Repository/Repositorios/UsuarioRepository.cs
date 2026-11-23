using Dapper;
using RetroGames.Model.Model;
using RetroGames.Repository.Conexao;

namespace RetroGames.Repository.Repositorios;

public class UsuarioRepository
{
    private readonly MySqlConnectionFactory _factory;

    public UsuarioRepository(MySqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        using var conexao = _factory.CreateConnection();

        return await conexao.QuerySingleOrDefaultAsync<Usuario>(
            """
            SELECT
                co_seq_usuario AS Id,
                no_usuario AS Nome,
                ds_email AS Email,
                ds_senha_hash AS SenhaHash,
                st_ativo AS Ativo,
                dt_cadastro AS DataCadastro,
                co_perfil AS IdPerfil
            FROM tb_usuario
            WHERE LOWER(ds_email) = LOWER(@Email)
            LIMIT 1;
            """,
            new { Email = email });
    }

    public async Task<int> InserirAsync(Usuario usuario)
    {
        using var conexao = _factory.CreateConnection();

        return await conexao.ExecuteScalarAsync<int>(
            """
            INSERT INTO tb_usuario
                (no_usuario, ds_email, ds_senha_hash, st_ativo, co_perfil)
            VALUES
                (@Nome, @Email, @SenhaHash, @Ativo, @IdPerfil);

            SELECT LAST_INSERT_ID();
            """,
            usuario);
    }
}
