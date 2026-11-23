using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Services.Servicos;
using RetroGames.WebApi.ApiModels.Plataformas;
using RetroGames.WebApi.ApiModels.Respostas;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly PlataformaService _plataformaService;

    public PlataformaController(PlataformaService plataformaService)
    {
        _plataformaService = plataformaService;
    }

    // GET: api/Plataforma
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var plataformas = await _plataformaService.ListarAsync();

        var resposta = plataformas.Select(p => new PlataformaResponse
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            Ativo = p.Ativo
        }).ToList();

        return Ok(ApiResponse<List<PlataformaResponse>>.Sucesso(
            resposta,
            "Plataformas consultadas com sucesso."));
    }

    // GET: api/Plataforma/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var plataforma = await _plataformaService.ObterPorIdAsync(id);

        var resposta = ConverterParaResponse(plataforma);

        return Ok(ApiResponse<PlataformaResponse>.Sucesso(
            resposta,
            "Plataforma consultada com sucesso."));
    }

    // POST: api/Plataforma
    [HttpPost]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Inserir(
        [FromBody] PlataformaRequest request)
    {
        var plataforma = new Plataforma
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Ativo = true
        };

        var id = await _plataformaService.InserirAsync(plataforma);

        var cadastrada = await _plataformaService.ObterPorIdAsync(id);

        var resposta = ApiResponse<PlataformaResponse>.Sucesso(
            ConverterParaResponse(cadastrada),
            "Plataforma cadastrada com sucesso.");

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id },
            resposta);
    }

    // PUT: api/Plataforma/{id}
    [HttpPut("{id:int}")]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Alterar(
        int id,
        [FromBody] PlataformaRequest request)
    {
        var plataforma = new Plataforma
        {
            Id = id,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Ativo = request.Ativo
        };

        await _plataformaService.AlterarAsync(plataforma);

        var atualizada = await _plataformaService.ObterPorIdAsync(id);

        return Ok(ApiResponse<PlataformaResponse>.Sucesso(
            ConverterParaResponse(atualizada),
            "Plataforma atualizada com sucesso."));
    }

    // DELETE: api/Plataforma/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _plataformaService.ExcluirAsync(id);

        return Ok(ApiResponse<object>.Sucesso(
            null,
            "Plataforma excluÃ­da com sucesso."));
    }

    private static PlataformaResponse ConverterParaResponse(
        Plataforma plataforma)
    {
        return new PlataformaResponse
        {
            Id = plataforma.Id,
            Nome = plataforma.Nome,
            Descricao = plataforma.Descricao,
            Ativo = plataforma.Ativo
        };
    }
}
