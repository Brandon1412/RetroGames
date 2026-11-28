using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Model.Constantes;
using RetroGames.Repository.Repositorios;
using RetroGames.Services.Excecoes;

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

        var emailNormalizado = email.Trim();
        var existente = await _repository.ObterPorEmailAsync(emailNormalizado);

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
            Nome = string.IsNullOrWhiteSpace(nome)
                ? "Administrador"
                : nome.Trim(),
            Email = emailNormalizado,
            Ativo = true,
            IdPerfil = PerfilIds.Administrador
        };

        usuario.SenhaHash = _hasher.HashPassword(usuario, senha);

        await _repository.InserirAsync(usuario);
    }

    public async Task CriarAdministradorAsync(
        string nome,
        string email,
        string senha)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Informe o nome do administrador.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Informe o e-mail do administrador.");

        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
            throw new ArgumentException(
                "A senha deve possuir pelo menos 8 caracteres.");

        var emailNormalizado = email.Trim();

        var existente = await _repository.ObterPorEmailAsync(emailNormalizado);

        if (existente != null)
        {
            throw new RegistroDuplicadoException(
                "Já existe um usuário cadastrado com esse e-mail.");
        }

        var usuario = new Usuario
        {
            Nome = nome.Trim(),
            Email = emailNormalizado,
            Ativo = true,
            IdPerfil = PerfilIds.Administrador
        };

        usuario.SenhaHash = _hasher.HashPassword(usuario, senha);

        await _repository.InserirAsync(usuario);
    }

    public async Task AlterarMinhaSenhaAsync(
        int idUsuario,
        string senhaAtual,
        string novaSenha)
    {
        if (idUsuario <= 0)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (string.IsNullOrWhiteSpace(senhaAtual))
            throw new ArgumentException("Informe sua senha atual.");

        if (string.IsNullOrWhiteSpace(novaSenha) || novaSenha.Length < 8)
            throw new ArgumentException(
                "A nova senha deve possuir pelo menos 8 caracteres.");

        if (senhaAtual == novaSenha)
            throw new ArgumentException(
                "A nova senha deve ser diferente da senha atual.");

        var usuario = await _repository.ObterPorIdAsync(idUsuario);

        if (usuario == null ||
            !usuario.Ativo ||
            usuario.IdPerfil != PerfilIds.Administrador)
        {
            throw new KeyNotFoundException(
                "Conta de administrador não encontrada.");
        }

        var resultado = _hasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            senhaAtual);

        if (resultado == PasswordVerificationResult.Failed)
        {
            throw new SenhaAtualIncorretaException(
                "A senha atual informada está incorreta.");
        }

        var novoHash = _hasher.HashPassword(usuario, novaSenha);

        var linhasAtualizadas = await _repository.AtualizarSenhaAsync(
            idUsuario,
            novoHash);

        if (linhasAtualizadas == 0)
        {
            throw new KeyNotFoundException(
                "Não foi possível atualizar a senha da conta.");
        }
    }
}
