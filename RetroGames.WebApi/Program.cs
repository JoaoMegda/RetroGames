using System.Text.Json;
using DotNetEnv;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuracao;
using RetroGames.WebApi.Filters;
using RetroGames.WebApi.Handlers;
using RetroGames.WebApi.Services;
using RetroGames.WebApi.Validators;

// O .env (raiz do repositório, fora do Git) vira variáveis de ambiente antes de a configuração ser montada.
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var stringConexao = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Defina ConnectionStrings__Default no arquivo .env.");

builder.Services
    .AddRepositories(stringConexao)
    .AddServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();

builder.Services
    .AddControllersWithViews(opcoes =>
    {
        opcoes.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        // O envelope substitui a falha de CSRF antes da conversão padrão para ProblemDetails.
        opcoes.Filters.Add<AntiforgeryEnvelopeFilter>(order: -3000);
        opcoes.Filters.Add<FluentValidationFilter>();
    })
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        opcoes.SuppressModelStateInvalidFilter = true;
        opcoes.InvalidModelStateResponseFactory = contexto =>
        {
            // O caminho do JSON identifica o campo que falhou na conversão, como $.anoLancamento.
            var erros = contexto.ModelState
                .Where(campo => campo.Value?.Errors.Count > 0 && campo.Key.StartsWith("$.", StringComparison.Ordinal))
                .Select(campo => JsonNamingPolicy.CamelCase.ConvertName(campo.Key[2..].Split(new[] { '.', '[' })[0]))
                .Distinct()
                .ToDictionary(campo => campo, _ => new[] { "Informe um valor válido." });

            return new ObjectResult(EnvelopeApiModel.Erro(
                ResponseCodes.MODEL_STATE_INVALIDO, erros.Count > 0 ? erros : null))
            { StatusCode = StatusCodes.Status400BadRequest };
        };
    });

builder.Services.AddValidatorsFromAssemblyContaining<InserirPlataformaRequestValidator>();

builder.Services.AdicionarAutenticacao(builder.Environment.IsDevelopment());
builder.Services.AdicionarAutorizacao();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler(_ => { });

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
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
