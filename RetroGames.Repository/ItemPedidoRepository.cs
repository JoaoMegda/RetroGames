using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class ItemPedidoRepository : RepositoryBase, IItemPedidoRepository
{
    public ItemPedidoRepository(RepositoryConfig configuracao) : base(configuracao) { }

    public async Task InserirAsync(ItemPedido item) =>
        await ExecutarGravacaoAsync(
            "INSERT INTO tb_item_pedido (qt_item, vl_unitario, co_pedido, co_jogo) " +
            "VALUES (@quantidade, @valorUnitario, @idPedido, @idJogo)",
            p =>
            {
                p.AddWithValue("@quantidade", item.Quantidade);
                p.AddWithValue("@valorUnitario", item.ValorUnitario);
                p.AddWithValue("@idPedido", item.IdPedido);
                p.AddWithValue("@idJogo", item.IdJogo);
            });

    public Task<List<ItemPedido>> ListarPorPedidosAsync(IReadOnlyCollection<int> idsPedidos)
    {
        if (idsPedidos.Count == 0)
            return Task.FromResult(new List<ItemPedido>());

        return ConsultarAsync(
            "SELECT co_seq_item_pedido, qt_item, vl_unitario, co_pedido, co_jogo " +
            $"FROM tb_item_pedido WHERE co_pedido IN ({NomesParametros("idPedido", idsPedidos.Count)}) " +
            "ORDER BY co_seq_item_pedido",
            p => AdicionarLista(p, "idPedido", idsPedidos),
            Mapear);
    }

    private static ItemPedido Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_seq_item_pedido"),
        Quantidade = leitor.GetInt32("qt_item"),
        ValorUnitario = leitor.GetDecimal("vl_unitario"),
        IdPedido = leitor.GetInt32("co_pedido"),
        IdJogo = leitor.GetInt32("co_jogo"),
    };
}
