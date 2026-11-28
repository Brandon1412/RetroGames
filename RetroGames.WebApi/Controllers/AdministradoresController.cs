using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Servicos;
using RetroGames.WebApi.ApiModels.Autenticacao;
using RetroGames.WebApi.ApiModels.Respostas;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SomenteAdministrador")]
public class AdministradoresController : ControllerBase
{
    private readonly AutenticacaoService _autenticacaoService;

    public AdministradoresController(
        AutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar(
        [FromBody] CadastrarAdministradorRequest request)
    {
        await _autenticacaoService.CriarAdministradorAsync(
            request.Nome,
            request.Email,
            request.Senha);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<object>.Sucesso(
                null,
                "Administrador cadastrado com sucesso."));
    }
}
