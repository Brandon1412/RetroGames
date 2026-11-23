using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _accessor;

    public UsuarioAtualService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public UsuarioAtual? Obter()
    {
        var usuario = _accessor.HttpContext?.User;

        if (usuario?.Identity?.IsAuthenticated != true)
            return null;

        return new UsuarioAtual
        {
            Id = int.TryParse(
                usuario.FindFirstValue(ClaimTypes.NameIdentifier),
                out var id) ? id : 0,

            Nome = usuario.Identity.Name ?? string.Empty,
            Email = usuario.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Perfil = usuario.FindFirstValue(ClaimTypes.Role) ?? string.Empty
        };
    }
}
