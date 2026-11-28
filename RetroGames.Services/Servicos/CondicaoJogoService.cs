using RetroGames.Model.Model;
using RetroGames.Repository.Repositorios;

namespace RetroGames.Services.Servicos;

public class CondicaoJogoService
{
    private readonly CondicaoJogoRepository _repository;

    public CondicaoJogoService(CondicaoJogoRepository repository)
    {
        _repository = repository;
    }

    public Task<List<CondicaoJogo>> ListarAsync()
    {
        return _repository.ListarAsync();
    }
}
