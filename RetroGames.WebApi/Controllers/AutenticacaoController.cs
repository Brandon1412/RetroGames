
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Constantes;
using RetroGames.Services.Servicos;
using RetroGames.WebApi.ApiModels.Autenticacao;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly AutenticacaoService _autenticacaoService;

    public AutenticacaoController(AutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Senha))
        {
            return BadRequest(new
            {
                codigo = 3,
                mensagem = "E-mail e senha são obrigatórios."
            });
        }

        var usuario = await _autenticacaoService.LoginAsync(
            request.Email,
            request.Senha
        );

        if (usuario == null)
        {
            return Unauthorized(new
            {
                codigo = 6,
                mensagem = "E-mail ou senha inválidos."
            });
        }

        var perfil = usuario.IdPerfil == PerfilIds.Administrador
            ? "Administrador"
            : "Cliente";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, perfil)
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identidade);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return Ok(new
        {
            codigo = 4,
            mensagem = "Login realizado com sucesso.",
            usuario = new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = perfil
            }
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return Ok(new
        {
            codigo = 5,
            mensagem = "Logout realizado com sucesso."
        });
    }

    [HttpGet("eu")]
    [Authorize]
    public IActionResult Eu()
    {
        var usuario = new
        {
            id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            nome = User.FindFirst(ClaimTypes.Name)?.Value,
            email = User.FindFirst(ClaimTypes.Email)?.Value,
            perfil = User.FindFirst(ClaimTypes.Role)?.Value
        };

        return Ok(new
        {
            codigo = 0,
            mensagem = "Usuário autenticado.",
            usuario
        });
    }
}
