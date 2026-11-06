using RetroGames.Model;

namespace RetroGames.Repository;

public interface IJogoGeneroRepository
{
    Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogo);
    Task<List<JogoGenero>> ListarPorGeneroAsync(int idGenero);
    Task InserirAsync(JogoGenero jogoGenero);
    Task ExcluirPorJogoAsync(int idJogo);
}
