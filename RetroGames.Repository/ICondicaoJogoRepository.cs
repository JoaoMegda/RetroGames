using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface ICondicaoJogoRepository
{
    Task<List<CondicaoJogo>> ListarAsync();
    Task<CondicaoJogo?> ObterPorIdAsync(int id);
}
