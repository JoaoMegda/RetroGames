using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuracao;

public static class AutorizacaoConfig
{
    public static void AdicionarAutorizacao(this IServiceCollection servicos)
    {
        servicos.AddAuthorization(opcoes =>
        {
            opcoes.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            opcoes.AddPolicy(Politicas.Administrador, politica => politica
                .RequireAuthenticatedUser()
                .RequireRole(Perfis.NomeAdministrador));
        });
    }
}
