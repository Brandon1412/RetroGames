using RetroGames.Model.Model;
using RetroGames.Repository.Repositorios;

namespace RetroGames.Services.Servicos;

public class GeneroService
{
    private readonly GeneroRepository _repository;

    public GeneroService(GeneroRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Genero>> ListarAtivosAsync()
    {
        return _repository.ListarAtivosAsync();
    }
}
