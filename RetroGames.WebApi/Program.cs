
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Interfaces;
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

// =====================================================
// CONFIGURACAO DO BANCO DE DADOS
// =====================================================

var connectionString = builder.Configuration
    .GetConnectionString("RetroGames")
    ?? throw new InvalidOperationException(
        "A string de conexao RetroGames nao foi configurada.");

builder.Services.AddSingleton(
    new MySqlConnectionFactory(connectionString));

// =====================================================
// CONFIGURACAO DOS SERVICOS
// =====================================================

builder.Services.AddScoped<PlataformaRepository>();

builder.Services.AddScoped<IPlataformaRepository>(sp =>
    sp.GetRequiredService<PlataformaRepository>());

builder.Services.AddScoped<PlataformaService>();

builder.Services.AddScoped<JogoRepository>();

builder.Services.AddScoped<IJogoRepository>(sp =>
    sp.GetRequiredService<JogoRepository>());

builder.Services.AddScoped<JogoService>();

builder.Services.AddScoped<GeneroRepository>();

builder.Services.AddScoped<IGeneroRepository>(sp =>
    sp.GetRequiredService<GeneroRepository>());

builder.Services.AddScoped<GeneroService>();

builder.Services.AddScoped<CondicaoJogoRepository>();

builder.Services.AddScoped<ICondicaoJogoRepository>(sp =>
    sp.GetRequiredService<CondicaoJogoRepository>());

builder.Services.AddScoped<CondicaoJogoService>();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<AutenticacaoService>();

builder.Services.AddScoped<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    IUsuarioAtualService,
    UsuarioAtualService>();

// =====================================================
// CONFIGURACAO DE HTTPS E PROXY
// =====================================================

// Permite que a API reconheca HTTPS quando estiver
// hospedada atras de um proxy reverso.

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    // Necessario em alguns servicos de hospedagem
    // que utilizam proxy reverso.
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// =====================================================
// CONFIGURACAO DE CORS
// =====================================================

var origemFrontend = builder.Configuration[
    "Frontend:Origin"]
    ?? "https://brandon1412.github.io";

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(origemFrontend)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// =====================================================
// CONFIGURACAO DA AUTENTICACAO
// =====================================================

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "retrogames.sessao";

        options.Cookie.HttpOnly = true;

        // Em producao, permite cookies entre origens.
        // Em desenvolvimento, permite funcionar localmente.
        options.Cookie.SameSite = builder.Environment.IsDevelopment()
            ? SameSiteMode.Lax
            : SameSiteMode.None;

        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;

        options.Cookie.IsEssential = true;

        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;

        // Retorna 401 em vez de redirecionar para uma
        // pagina de login que nao existe na API.
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            return context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Erro(
                    ResponseCodes.NAO_AUTENTICADO,
                    "Autenticacao necessaria."));
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode =
                StatusCodes.Status403Forbidden;

            return context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Erro(
                    ResponseCodes.ACESSO_NEGADO,
                    "Acesso permitido somente a administradores."));
        };
    });

// =====================================================
// CONFIGURACAO DA AUTORIZACAO
// =====================================================

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

    options.AddPolicy("SomenteAdministrador", policy =>
        policy.RequireRole("Administrador"));
});

// =====================================================
// VALIDACOES
// =====================================================

builder.Services.AddScoped<FluentValidationFilter>();

builder.Services
    .AddValidatorsFromAssemblyContaining<
        PlataformaRequestValidator>();

builder.Services
    .AddValidatorsFromAssemblyContaining<
        JogoRequestValidator>();

// =====================================================
// CONFIGURACAO DOS CONTROLLERS
// =====================================================

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

// =====================================================
// INICIALIZACAO DA APLICACAO
// =====================================================

var app = builder.Build();

app.UseForwardedHeaders();

app.UseExceptionHandler();

// Documentacao da API somente em desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

// Redireciona HTTP para HTTPS.
app.UseHttpsRedirection();

// CORS precisa ser aplicado antes da autenticacao.
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// =====================================================
// CRIACAO DO ADMINISTRADOR INICIAL
// =====================================================

using (var scope = app.Services.CreateScope())
{
    var configuracao = scope.ServiceProvider
        .GetRequiredService<IConfiguration>();

    var emailAdmin = configuracao["AdminInicial:Email"];
    var senhaAdmin = configuracao["AdminInicial:Senha"];

    var nomeAdmin = configuracao["AdminInicial:Nome"]
        ?? "Administrador";

    var autenticacao = scope.ServiceProvider
        .GetRequiredService<AutenticacaoService>();

    await autenticacao.CriarAdministradorInicialAsync(
        emailAdmin ?? "",
        senhaAdmin ?? "",
        nomeAdmin);
}

app.Run();
