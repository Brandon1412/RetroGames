namespace RetroGames.WebApi.ApiModels.Respostas;

public static class ResponseCodes
{
    public const int SUCESSO = 0;
    public const int ERRO_INTERNO = 1;
    public const int DADOS_INVALIDOS = 3;

    public const int LOGIN_SUCESSO = 4;
    public const int LOGOUT_SUCESSO = 5;
    public const int NAO_AUTENTICADO = 6;
    public const int ACESSO_NEGADO = 7;

    public const int NAO_ENCONTRADO = 100;
    public const int REGISTRO_DUPLICADO = 101;
    public const int PLATAFORMA_POSSUI_JOGOS = 102;
    public const int RELACIONAMENTO_NAO_ENCONTRADO = 103;
}
