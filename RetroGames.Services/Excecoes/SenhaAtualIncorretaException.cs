namespace RetroGames.Services.Excecoes;

public class SenhaAtualIncorretaException : Exception
{
    public SenhaAtualIncorretaException(string message)
        : base(message)
    {
    }
}
