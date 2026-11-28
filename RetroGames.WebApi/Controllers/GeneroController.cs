using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Servicos;
using RetroGames.WebApi.ApiModels.Generos;
using RetroGames.WebApi.ApiModels.Respostas;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SomenteAdministrador")]
public class GeneroController : ControllerBase
{
    private readonly GeneroService _service;

    public GeneroController(GeneroService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var generos = await _service.ListarAtivosAsync();

        var resposta = generos.Select(g => new GeneroResponse
        {
            Id = g.Id,
            Nome = g.Nome,
            Descricao = g.Descricao
        }).ToList();

        return Ok(ApiResponse<List<GeneroResponse>>.Sucesso(
            resposta,
            "Generos consultados com sucesso."));
    }
}
