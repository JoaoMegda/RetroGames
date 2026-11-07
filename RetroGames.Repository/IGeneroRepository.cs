using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IGeneroRepository
{
    Task<List<Genero>> ListarAsync();
    Task<List<Genero>> ListarPorIdsAsync(IReadOnlyCollection<int> ids);
}
