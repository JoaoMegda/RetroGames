using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    Task<List<Jogo>> ListarAsync(JogoFiltro filtro);
    Task<int> ContarAsync(JogoFiltro filtro);
    Task<Jogo?> ObterPorIdAsync(int id);
    Task<int> ContarPorPlataformaAsync(int idPlataforma);
    Task<int> InserirAsync(Jogo jogo);
    Task<bool> AlterarAsync(Jogo jogo);
    Task<bool> DesativarAsync(int id);
}
