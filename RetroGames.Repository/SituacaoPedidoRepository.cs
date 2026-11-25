using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class SituacaoPedidoRepository : RepositoryBase, ISituacaoPedidoRepository
{
    private const string Colunas = "co_situacao_pedido, ds_situacao_pedido";

    public SituacaoPedidoRepository(RepositoryConfig configuracao) : base(configuracao) { }

    public Task<SituacaoPedido?> ObterPorIdAsync(int id) =>
        ConsultarUmAsync(
            $"SELECT {Colunas} FROM tb_situacao_pedido WHERE co_situacao_pedido = @id",
            p => p.AddWithValue("@id", id),
            Mapear);

    public Task<List<SituacaoPedido>> ListarAsync() =>
        ConsultarAsync($"SELECT {Colunas} FROM tb_situacao_pedido ORDER BY co_situacao_pedido", null, Mapear);

    private static SituacaoPedido Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_situacao_pedido"),
        Descricao = leitor.GetString("ds_situacao_pedido"),
    };
}
