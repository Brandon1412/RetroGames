using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Repositorios;
using RetroGames.Services.Servicos;
using RetroGames.WebApi.ApiModels.Respostas;
using RetroGames.WebApi.Filtros;
using RetroGames.WebApi.TratamentoErros;
using RetroGames.WebApi.Validadores.Plataformas;
using RetroGames.WebApi.Validadores.Jogos;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("RetroGames")
    ?? throw new InvalidOperationException(
        "A string de conexao RetroGames nao foi configurada.");

builder.Services.AddSingleton(
    new MySqlConnectionFactory(connectionString));

builder.Services.AddScoped<PlataformaRepository>();
builder.Services.AddScoped<PlataformaService>();

builder.Services.AddScoped<JogoRepository>();
builder.Services.AddScoped<JogoService>();

builder.Services.AddScoped<FluentValidationFilter>();

builder.Services.AddValidatorsFromAssemblyContaining<PlataformaRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<JogoRequestValidator>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<FluentValidationFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var erros = context.ModelState
            .Where(item => item.Value?.Errors.Count > 0)
            .ToDictionary(
                item => item.Key,
                item => item.Value!.Errors
                    .Select(erro => erro.ErrorMessage)
                    .ToArray());

        var resposta = new ApiResponse<object>
        {
            ResponseCode = ResponseCodes.DADOS_INVALIDOS,
            Message = "Os dados informados sao invalidos.",
            Errors = erros
        };

        return new BadRequestObjectResult(resposta);
    };
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
