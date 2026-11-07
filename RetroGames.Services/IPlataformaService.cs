using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IPlataformaService
{
    Task<List<Plataforma>> ListarAsync();
    Task<Plataforma> ObterAsync(int id);
    Task<Plataforma> InserirAsync(Plataforma plataforma);
    Task<Plataforma> AlterarAsync(Plataforma plataforma);
    Task ExcluirAsync(int id);
}
