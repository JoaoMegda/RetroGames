using Microsoft.AspNetCore.Identity;
using RetroGames.Model;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.Configuracao;

public static class InjecaoDependenciaExtensoes
{
    public static IServiceCollection AddRetroGames(this IServiceCollection servicos, string stringConexao)
    {
        servicos.AddSingleton(new ConfiguracaoBanco(stringConexao));

        servicos.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        servicos.AddScoped<IGeneroRepository, GeneroRepository>();
        servicos.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        servicos.AddScoped<IJogoRepository, JogoRepository>();
        servicos.AddScoped<IPerfilRepository, PerfilRepository>();
        servicos.AddScoped<IPlataformaRepository, PlataformaRepository>();
        servicos.AddScoped<IUsuarioRepository, UsuarioRepository>();
        servicos.AddScoped<IRepositorySet, RepositorySet>();

        servicos.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        servicos.AddScoped<IAutenticacaoService, AutenticacaoService>();
        servicos.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        servicos.AddScoped<IGeneroService, GeneroService>();
        servicos.AddScoped<IJogoService, JogoService>();
        servicos.AddScoped<IPlataformaService, PlataformaService>();

        return servicos;
    }
}
