
using MySqlConnector;

namespace RetroGames.Repository.Conexao;

public class MySqlConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
