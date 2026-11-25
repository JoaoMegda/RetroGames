using RetroGames.Model;
using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<ListaPaginadaServiceModel<JogoDetalheServiceModel>> ListarAsync(JogoFiltroServiceModel filtro);
    Task<JogoDetalheServiceModel> ObterAsync(int id);
    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGenero);
    Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGenero, bool manterEstoque = false);
    Task DesativarAsync(int id);
}
