using RetroGames.Model;

namespace RetroGames.Repository;

public interface IPerfilRepository
{
    Task<Perfil?> ObterPorIdAsync(int id);
}
