
using RetroGames.Model.Model;
using RetroGames.Repository.Repositorios;
using RetroGames.Services.Excecoes;

namespace RetroGames.Services.Servicos;

public class PlataformaService
{
    private readonly PlataformaRepository _plataformaRepository;

    public PlataformaService(PlataformaRepository plataformaRepository)
    {
        _plataformaRepository = plataformaRepository;
    }

    // Lista todas as plataformas.
    public async Task<List<Plataforma>> ListarAsync()
    {
        return await _plataformaRepository.ListarAsync();
    }

    // Busca uma plataforma pelo ID.
    public async Task<Plataforma> ObterPorIdAsync(int id)
    {
        var plataforma = await _plataformaRepository.ObterPorIdAsync(id);

        if (plataforma == null)
        {
            throw new KeyNotFoundException(
                $"A plataforma com ID {id} não foi encontrada.");
        }

        return plataforma;
    }

    // Cadastra uma nova plataforma.
    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        var existente = await _plataformaRepository.ListarAsync();

        if (existente.Any(p =>
            p.Nome.Equals(plataforma.Nome,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new RegistroDuplicadoException(
                "Já existe uma plataforma cadastrada com esse nome.");
        }

        plataforma.Ativo = true;

        return await _plataformaRepository.InserirAsync(plataforma);
    }

    // Altera uma plataforma existente.
    public async Task AlterarAsync(Plataforma plataforma)
    {
        var existente = await _plataformaRepository.ObterPorIdAsync(
            plataforma.Id);

        if (existente == null)
        {
            throw new KeyNotFoundException(
                $"A plataforma com ID {plataforma.Id} não foi encontrada.");
        }

        var plataformas = await _plataformaRepository.ListarAsync();

        if (plataformas.Any(p =>
            p.Id != plataforma.Id &&
            p.Nome.Equals(plataforma.Nome,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new RegistroDuplicadoException(
                "Já existe outra plataforma cadastrada com esse nome.");
        }

        await _plataformaRepository.AlterarAsync(plataforma);
    }

    // Exclui uma plataforma somente se não possuir jogos.
    public async Task ExcluirAsync(int id)
    {
        var plataforma = await _plataformaRepository.ObterPorIdAsync(id);

        if (plataforma == null)
        {
            throw new KeyNotFoundException(
                $"A plataforma com ID {id} não foi encontrada.");
        }

        var possuiJogos = await _plataformaRepository.PossuiJogosAsync(id);

        if (possuiJogos)
        {
            throw new PlataformaPossuiJogosException(
                "Não é possível excluir uma plataforma que possui jogos cadastrados.");
        }

        await _plataformaRepository.ExcluirAsync(id);
    }
}
