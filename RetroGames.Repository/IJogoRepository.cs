using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    Task<List<Jogo>> ListarAsync(JogoFiltro filtro);
    Task<int> ContarAsync(JogoFiltro filtro);
    Task<Jogo?> ObterPorIdAsync(int id);
    Task<Jogo?> ObterParaCompraAsync(int id);
    Task<List<Jogo>> ListarPorIdsAsync(IReadOnlyCollection<int> ids);
    Task<bool> BaixarEstoqueAsync(int id, int quantidade);
    Task<int> InserirAsync(Jogo jogo);
    Task<bool> AlterarAsync(Jogo jogo, bool manterEstoque = false);
    Task<bool> DesativarAsync(int id);
}
