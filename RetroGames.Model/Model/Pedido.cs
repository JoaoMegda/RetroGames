namespace RetroGames.Model.Model;

public class Pedido
{
    /// <summary>co_seq_pedido</summary>
    public int Id { get; set; }

    /// <summary>dt_pedido</summary>
    public DateTime Data { get; set; }

    /// <summary>vl_total</summary>
    public decimal Total { get; set; }

    /// <summary>co_usuario</summary>
    public int IdUsuario { get; set; }

    /// <summary>co_situacao_pedido</summary>
    public int IdSituacaoPedido { get; set; }
}
