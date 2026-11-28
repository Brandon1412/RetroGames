using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoRepository
{
    Task<List<Jogo>> ListarAsync();

    Task<Jogo?> ObterPorIdAsync(int id);

    Task<bool> ExistePlataformaAtivaAsync(int id);

    Task<bool> ExisteCondicaoAsync(int id);

    Task<bool> GenerosExistemAsync(List<int> ids);

    Task<int> InserirAsync(Jogo jogo);

    Task AlterarAsync(Jogo jogo);

    Task ExcluirLogicamenteAsync(int id);
}
