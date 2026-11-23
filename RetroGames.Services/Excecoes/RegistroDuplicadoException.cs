
namespace RetroGames.Services.Excecoes;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(string message)
        : base(message)
    {
    }
}
