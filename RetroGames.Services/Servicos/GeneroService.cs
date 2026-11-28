using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Services.Servicos;

public class GeneroService
{
    private readonly IGeneroRepository _repository;

    public GeneroService(IGeneroRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Genero>> ListarAtivosAsync()
    {
        return _repository.ListarAtivosAsync();
    }
}
