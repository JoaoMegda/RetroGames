using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPlataformaRepository
{
    Task<List<Plataforma>> ListarAsync();
    Task<Plataforma?> ObterPorIdAsync(int id);
    Task<Plataforma?> ObterPorNomeAsync(string nome);
    Task<int> InserirAsync(Plataforma plataforma);
    Task<bool> AlterarAsync(Plataforma plataforma);
    Task<bool> ExcluirAsync(int id);
}
