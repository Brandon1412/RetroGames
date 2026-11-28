using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Services.Servicos;

public class CondicaoJogoService
{
    private readonly ICondicaoJogoRepository _repository;

    public CondicaoJogoService(ICondicaoJogoRepository repository)
    {
        _repository = repository;
    }

    public Task<List<CondicaoJogo>> ListarAsync()
    {
        return _repository.ListarAsync();
    }
}
