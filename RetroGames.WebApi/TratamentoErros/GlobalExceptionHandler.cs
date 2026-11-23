
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Excecoes;
using RetroGames.WebApi.ApiModels.Respostas;

namespace RetroGames.WebApi.TratamentoErros;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, responseCode, message) = exception switch
        {
            KeyNotFoundException =>
                (StatusCodes.Status404NotFound,
                 ResponseCodes.NAO_ENCONTRADO,
                 exception.Message),

            RegistroDuplicadoException =>
                (StatusCodes.Status409Conflict,
                 ResponseCodes.REGISTRO_DUPLICADO,
                 exception.Message),

            PlataformaPossuiJogosException =>
                (StatusCodes.Status409Conflict,
                 ResponseCodes.PLATAFORMA_POSSUI_JOGOS,
                 exception.Message),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 ResponseCodes.ERRO_INTERNO,
                 "Ocorreu um erro interno no servidor.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Erro inesperado na API.");
        }

        httpContext.Response.StatusCode = statusCode;

        var resposta = ApiResponse<object>.Erro(
            responseCode,
            message);

        await httpContext.Response.WriteAsJsonAsync(
            resposta,
            cancellationToken);

        return true;
    }
}
