using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Servicos;
using RetroGames.WebApi.ApiModels.CondicoesJogo;
using RetroGames.WebApi.ApiModels.Respostas;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SomenteAdministrador")]
public class CondicaoJogoController : ControllerBase
{
    private readonly CondicaoJogoService _service;

    public CondicaoJogoController(CondicaoJogoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var condicoes = await _service.ListarAsync();

        var resposta = condicoes.Select(c => new CondicaoJogoResponse
        {
            Id = c.Id,
            Descricao = c.Descricao
        }).ToList();

        return Ok(ApiResponse<List<CondicaoJogoResponse>>.Sucesso(
            resposta,
            "Condicoes consultadas com sucesso."));
    }
}
