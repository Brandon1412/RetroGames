using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IPlataformaRepository
{
    Task<bool> PossuiJogosAsync(int idPlataforma);

    Task<List<Plataforma>> ListarAsync();

    Task<Plataforma?> ObterPorIdAsync(int id);

    Task<int> InserirAsync(Plataforma plataforma);

    Task<bool> AlterarAsync(Plataforma plataforma);

    Task<bool> ExcluirAsync(int id);
}