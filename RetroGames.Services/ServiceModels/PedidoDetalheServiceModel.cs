namespace RetroGames.Services.ServiceModels;

public class PedidoDetalheServiceModel
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<ItemPedidoDetalheServiceModel> Itens { get; set; } = [];
}

public class ItemPedidoDetalheServiceModel
{
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}
