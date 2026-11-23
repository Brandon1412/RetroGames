
namespace RetroGames.WebApi.ApiModels.Respostas;

public static class ResponseCodes
{
    // Operações realizadas com sucesso.
    public const int SUCESSO = 0;

    // Erro interno inesperado.
    public const int ERRO_INTERNO = 1;

    // Dados enviados em formato inválido.
    public const int DADOS_INVALIDOS = 3;

    // Recurso não encontrado.
    public const int NAO_ENCONTRADO = 100;

    // Registro duplicado.
    public const int REGISTRO_DUPLICADO = 101;

    // Não é possível excluir uma plataforma vinculada a jogos.
    public const int PLATAFORMA_POSSUI_JOGOS = 102;

    // Registro relacionado não encontrado.
    public const int RELACIONAMENTO_NAO_ENCONTRADO = 103;
}
