using RetroGames.Model;

namespace RetroGames.Services;

public interface ICondicaoJogoService
{
    Task<List<CondicaoJogo>> ListarAsync();
}
