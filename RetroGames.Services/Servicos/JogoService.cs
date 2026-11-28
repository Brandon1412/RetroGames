
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Excecoes;

namespace RetroGames.Services.Servicos;

public class JogoService
{
    private readonly IJogoRepository _repository;

    public JogoService(IJogoRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Jogo>> ListarAsync()
    {
        return _repository.ListarAsync();
    }

    public async Task<Jogo> ObterPorIdAsync(int id)
    {
        var jogo = await _repository.ObterPorIdAsync(id);

        if (jogo == null)
            throw new KeyNotFoundException(
                $"O jogo com ID {id} nao foi encontrado.");

        return jogo;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        await ValidarRelacionamentosAsync(jogo);

        jogo.Ativo = true;

        return await _repository.InserirAsync(jogo);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        var existente = await _repository.ObterPorIdAsync(jogo.Id);

        if (existente == null)
            throw new KeyNotFoundException(
                $"O jogo com ID {jogo.Id} nao foi encontrado.");

        await ValidarRelacionamentosAsync(jogo);

        await _repository.AlterarAsync(jogo);
    }

    public async Task ExcluirAsync(int id)
    {
        var existente = await _repository.ObterPorIdAsync(id);

        if (existente == null)
            throw new KeyNotFoundException(
                $"O jogo com ID {id} nao foi encontrado.");

        await _repository.ExcluirLogicamenteAsync(id);
    }

    private async Task ValidarRelacionamentosAsync(Jogo jogo)
    {
        if (!await _repository.ExistePlataformaAtivaAsync(jogo.IdPlataforma))
            throw new RelacionamentoNaoEncontradoException(
                "A plataforma informada nao existe ou esta inativa.");

        if (!await _repository.ExisteCondicaoAsync(jogo.IdCondicaoJogo))
            throw new RelacionamentoNaoEncontradoException(
                "A condicao do jogo informada nao existe.");

        jogo.IdsGeneros = jogo.IdsGeneros.Distinct().ToList();

        if (jogo.IdsGeneros.Count == 0 ||
            !await _repository.GenerosExistemAsync(jogo.IdsGeneros))
        {
            throw new RelacionamentoNaoEncontradoException(
                "Um ou mais generos informados nao existem.");
        }
    }
}
