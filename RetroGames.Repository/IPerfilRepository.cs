using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPerfilRepository
{
    Task<Perfil?> ObterPorIdAsync(int id);
}
