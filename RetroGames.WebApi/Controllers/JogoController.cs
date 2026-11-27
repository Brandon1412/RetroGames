
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Services.Servicos;
using RetroGames.WebApi.ApiModels.Jogos;
using RetroGames.WebApi.ApiModels.Respostas;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly JogoService _service;

    public JogoController(JogoService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var administrador = User.IsInRole("Administrador");
        var jogos = await _service.ListarAsync();

        if (!administrador)
            jogos = jogos.Where(j => j.Ativo).ToList();

        return Ok(ApiResponse<List<JogoResponse>>.Sucesso(
            jogos.Select(Converter).ToList(),
            "Jogos consultados com sucesso."));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var jogo = await _service.ObterPorIdAsync(id);

        if (!jogo.Ativo && !User.IsInRole("Administrador"))
        {
            return NotFound(ApiResponse<object>.Erro(
                ResponseCodes.NAO_ENCONTRADO,
                "Jogo não encontrado."));
        }

        return Ok(ApiResponse<JogoResponse>.Sucesso(
            Converter(jogo),
            "Jogo consultado com sucesso."));
    }

    [HttpPost]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Inserir([FromBody] JogoRequest request)
    {
        var jogo = Converter(request);
        var id = await _service.InserirAsync(jogo);
        var cadastrado = await _service.ObterPorIdAsync(id);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id },
            ApiResponse<JogoResponse>.Sucesso(
                Converter(cadastrado),
                "Jogo cadastrado com sucesso."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Alterar(
        int id,
        [FromBody] JogoRequest request)
    {
        var jogo = Converter(request);
        jogo.Id = id;

        await _service.AlterarAsync(jogo);
        var atualizado = await _service.ObterPorIdAsync(id);

        return Ok(ApiResponse<JogoResponse>.Sucesso(
            Converter(atualizado),
            "Jogo atualizado com sucesso."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _service.ExcluirAsync(id);

        return Ok(ApiResponse<object>.Sucesso(
            null,
            "Jogo desativado com sucesso."));
    }

    private static Jogo Converter(JogoRequest request)
    {
        return new Jogo
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            AnoLancamento = request.AnoLancamento,
            Preco = request.Preco,
            Estoque = request.Estoque,
            IdPlataforma = request.IdPlataforma,
            IdCondicaoJogo = request.IdCondicaoJogo,
            IdsGeneros = request.IdsGeneros,
            Ativo = request.Ativo
        };
    }

    private static JogoResponse Converter(Jogo jogo)
    {
        return new JogoResponse
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            AnoLancamento = jogo.AnoLancamento,
            Preco = jogo.Preco,
            Estoque = jogo.Estoque,
            Ativo = jogo.Ativo,
            DataCadastro = jogo.DataCadastro,
            IdPlataforma = jogo.IdPlataforma,
            IdCondicaoJogo = jogo.IdCondicaoJogo,
            IdsGeneros = jogo.IdsGeneros
        };
    }
}
