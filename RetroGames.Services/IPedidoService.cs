using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public interface IPedidoService
{
    Task<PedidoDetalheServiceModel> InserirAsync(IReadOnlyCollection<InserirItemPedidoServiceModel?>? itens);
    Task<List<PedidoDetalheServiceModel>> ListarMeusAsync();
}
