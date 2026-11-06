using RetroGames.Model;

namespace RetroGames.Services;

public interface IGeneroService
{
    Task<List<Genero>> ListarAsync();
}
