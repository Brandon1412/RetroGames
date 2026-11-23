namespace RetroGames.Services.Interfaces;

public class UsuarioAtual
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
}

public interface IUsuarioAtualService
{
    UsuarioAtual? Obter();
}
