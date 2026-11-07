using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuracao;
using RetroGames.WebApi.Filters;
using RetroGames.WebApi.Handlers;
using RetroGames.WebApi.Validators;

// O .env (raiz do repositório, fora do Git) vira variáveis de ambiente antes de a configuração ser montada.
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var stringConexao = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Defina ConnectionStrings__Default no arquivo .env.");

builder.Services.AddRetroGames(stringConexao);

builder.Services
    .AddControllers(opcoes =>
    {
        opcoes.Filters.Add<FluentValidationFilter>();
        opcoes.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); // RS02
    })
    .ConfigureApiBehaviorOptions(opcoes => opcoes.SuppressModelStateInvalidFilter = true);

builder.Services.AddValidatorsFromAssemblyContaining<InserirPlataformaRequestValidator>();

builder.Services.AddAntiforgery(opcoes =>
{
    opcoes.HeaderName = "X-CSRF-TOKEN";
    opcoes.Cookie.Name = "retrogames.antiforgery";
    opcoes.Cookie.SameSite = SameSiteMode.Strict;
    opcoes.Cookie.HttpOnly = true;
});

// RS01: cookie HttpOnly, SameSite=Strict, expiração deslizante de 2 horas
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opcoes =>
    {
        opcoes.Cookie.Name = "retrogames.sessao";
        opcoes.Cookie.HttpOnly = true;
        opcoes.Cookie.SameSite = SameSiteMode.Strict;
        opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
        opcoes.SlidingExpiration = true;
        opcoes.Events.OnRedirectToLogin = contexto =>
        {
            contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return contexto.Response.WriteAsJsonAsync(EnvelopeApiModel.Erro(ResponseCodes.NAO_AUTENTICADO));
        };
        opcoes.Events.OnRedirectToAccessDenied = contexto =>
        {
            contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
            return contexto.Response.WriteAsJsonAsync(EnvelopeApiModel.Erro(ResponseCodes.ACESSO_NEGADO));
        };
    });

// RS03
builder.Services.AddAuthorization(opcoes =>
    opcoes.AddPolicy("SomenteAdministrador", politica => politica.RequireRole(Perfis.NomeAdministrador)));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes => opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames v1"));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Cria o administrador inicial (se ainda não existir) com as credenciais do .env. Exige o MySQL no ar.
using (var escopo = app.Services.CreateScope())
{
    var email = builder.Configuration["AdminInicial:Email"];
    var senha = builder.Configuration["AdminInicial:Senha"];
    if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(senha))
        await escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>()
            .GarantirAdministradorInicialAsync(email, senha);
}

app.Run();
