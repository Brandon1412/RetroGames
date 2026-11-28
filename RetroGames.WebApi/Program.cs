using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Repositorios;
using RetroGames.Services.Interfaces;
using RetroGames.Services.Servicos;
using RetroGames.WebApi.ApiModels.Respostas;
using RetroGames.WebApi.Filtros;
using RetroGames.WebApi.Services;
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

builder.Services.AddScoped<GeneroRepository>();
builder.Services.AddScoped<GeneroService>();

builder.Services.AddScoped<CondicaoJogoRepository>();
builder.Services.AddScoped<CondicaoJogoService>();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<AutenticacaoService>();

builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "retrogames.sessao";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.IsEssential = true;

        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Erro(
                    ResponseCodes.NAO_AUTENTICADO,
                    "Autenticacao necessaria."));
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Erro(
                    ResponseCodes.ACESSO_NEGADO,
                    "Acesso permitido somente a administradores."));
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("SomenteAdministrador", policy =>
        policy.RequireRole("Administrador"));
});

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
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Cria o administrador inicial somente se o e-mail ainda nao existir.
using (var scope = app.Services.CreateScope())
{
    var configuracao = scope.ServiceProvider
        .GetRequiredService<IConfiguration>();

    var emailAdmin = configuracao["AdminInicial:Email"];
    var senhaAdmin = configuracao["AdminInicial:Senha"];
    var nomeAdmin = configuracao["AdminInicial:Nome"] ?? "Administrador";

    var autenticacao = scope.ServiceProvider
        .GetRequiredService<AutenticacaoService>();

    await autenticacao.CriarAdministradorInicialAsync(
        emailAdmin ?? "",
        senhaAdmin ?? "",
        nomeAdmin);
}

app.Run();
