using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PedidoRepository : RepositoryBase, IPedidoRepository
{
    public PedidoRepository(RepositoryConfig configuracao) : base(configuracao) { }

    public async Task<int> InserirAsync(Pedido pedido)
    {
        var (idGerado, _) = await ExecutarGravacaoAsync(
            "INSERT INTO tb_pedido (dt_pedido, vl_total, co_usuario, co_situacao_pedido) " +
            "VALUES (@data, @total, @idUsuario, @idSituacaoPedido)",
            p =>
            {
                p.AddWithValue("@data", pedido.Data);
                p.AddWithValue("@total", pedido.Total);
                p.AddWithValue("@idUsuario", pedido.IdUsuario);
                p.AddWithValue("@idSituacaoPedido", pedido.IdSituacaoPedido);
            });

        return (int)idGerado;
    }

    public Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario) =>
        ConsultarAsync(
            "SELECT co_seq_pedido, dt_pedido, vl_total, co_usuario, co_situacao_pedido " +
            "FROM tb_pedido WHERE co_usuario = @idUsuario ORDER BY dt_pedido DESC, co_seq_pedido DESC",
            p => p.AddWithValue("@idUsuario", idUsuario),
            Mapear);

    private static Pedido Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_seq_pedido"),
        Data = DateTime.SpecifyKind(leitor.GetDateTime("dt_pedido"), DateTimeKind.Utc),
        Total = leitor.GetDecimal("vl_total"),
        IdUsuario = leitor.GetInt32("co_usuario"),
        IdSituacaoPedido = leitor.GetInt32("co_situacao_pedido"),
    };
}
