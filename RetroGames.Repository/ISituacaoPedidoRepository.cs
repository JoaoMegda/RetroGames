using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface ISituacaoPedidoRepository
{
    Task<SituacaoPedido?> ObterPorIdAsync(int id);
    Task<List<SituacaoPedido>> ListarAsync();
}
