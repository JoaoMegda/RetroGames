using RetroGames.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<ListaPaginadaServiceModel<JogoDetalheServiceModel>> ListarAsync(JogoFiltroServiceModel filtro, bool ehAdministrador);
    Task<JogoDetalheServiceModel> ObterAsync(int id, bool ehAdministrador);
    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGenero);
    Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGenero);
    Task DesativarAsync(int id);
}
