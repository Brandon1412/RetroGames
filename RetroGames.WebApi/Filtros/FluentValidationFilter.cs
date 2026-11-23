
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels.Respostas;

namespace RetroGames.WebApi.Filtros;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var erros = new Dictionary<string, List<string>>();

        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento == null)
            {
                continue;
            }

            var tipoValidador = typeof(IValidator<>)
                .MakeGenericType(argumento.GetType());

            var validador = context.HttpContext.RequestServices
                .GetService(tipoValidador);

            if (validador is not IValidator validator)
            {
                continue;
            }

            var resultado = await validator.ValidateAsync(
                new ValidationContext<object>(argumento),
                context.HttpContext.RequestAborted);

            foreach (var erro in resultado.Errors)
            {
                if (!erros.ContainsKey(erro.PropertyName))
                {
                    erros[erro.PropertyName] = new List<string>();
                }

                erros[erro.PropertyName].Add(erro.ErrorMessage);
            }
        }

        if (erros.Count > 0)
        {
            var resposta = new ApiResponse<object>
            {
                ResponseCode = ResponseCodes.DADOS_INVALIDOS,
                Message = "Os dados informados são inválidos.",
                Errors = erros.ToDictionary(
                    erro => erro.Key,
                    erro => erro.Value.ToArray())
            };

            context.Result = new BadRequestObjectResult(resposta);
            return;
        }

        await next();
    }
}
