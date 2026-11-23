namespace RetroGames.Services.Excecoes;

public class RelacionamentoNaoEncontradoException : Exception
{
    public RelacionamentoNaoEncontradoException(string message)
        : base(message)
    {
    }
}
