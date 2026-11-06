using RetroGames.Model;

namespace RetroGames.Repository;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<int> InserirAsync(Usuario usuario);
}
