namespace RetroGames.WebApi.ApiModels;

public class InserirPedidoRequestApiModel
{
    public List<InserirItemPedidoRequestApiModel?>? Itens { get; set; }
}

public class InserirItemPedidoRequestApiModel
{
    public int IdJogo { get; set; }
    public int Quantidade { get; set; }
}

public class ConsultarPedidoResponseApiModel
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<ConsultarItemPedidoResponseApiModel> Itens { get; set; } = [];
}

public class ConsultarItemPedidoResponseApiModel
{
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}
