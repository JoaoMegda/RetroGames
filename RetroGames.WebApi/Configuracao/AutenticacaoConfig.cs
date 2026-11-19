using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuracao;

public static class AutenticacaoConfig
{
    public static void AdicionarAutenticacao(this IServiceCollection servicos, bool emDesenvolvimento)
    {
        servicos.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;
                opcoes.Cookie.SecurePolicy = emDesenvolvimento
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
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
    }
}
