using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Model.Constantes;
using RetroGames.Repository.Repositorios;

namespace RetroGames.Services.Servicos;

public class AutenticacaoService
{
    private readonly UsuarioRepository _repository;
    private readonly IPasswordHasher<Usuario> _hasher;

    public AutenticacaoService(
        UsuarioRepository repository,
        IPasswordHasher<Usuario> hasher)
    {
        _repository = repository;
        _hasher = hasher;
    }

    public async Task<Usuario?> LoginAsync(string email, string senha)
    {
        var usuario = await _repository.ObterPorEmailAsync(email.Trim());

        if (usuario == null || !usuario.Ativo)
            return null;

        var resultado = _hasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            senha);

        if (resultado == PasswordVerificationResult.Failed)
            return null;

        return usuario;
    }

    public async Task CriarAdministradorInicialAsync(
        string email,
        string senha,
        string nome)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException(
                "Configure AdminInicial__Email e AdminInicial__Senha.");
        }

        var existente = await _repository.ObterPorEmailAsync(email);

        if (existente != null)
        {
            if (existente.IdPerfil != PerfilIds.Administrador)
            {
                throw new InvalidOperationException(
                    "O e-mail configurado para administrador já pertence a outro perfil.");
            }

            return;
        }

        var usuario = new Usuario
        {
            Nome = string.IsNullOrWhiteSpace(nome) ? "Administrador" : nome,
            Email = email.Trim(),
            Ativo = true,
            IdPerfil = PerfilIds.Administrador
        };

        usuario.SenhaHash = _hasher.HashPassword(usuario, senha);

        await _repository.InserirAsync(usuario);
    }
}
