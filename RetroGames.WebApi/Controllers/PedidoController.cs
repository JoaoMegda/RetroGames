using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService _service;

    public PedidoController(IPedidoService service) => _service = service;

    [HttpPost]
    [ProducesResponseType<RetroGamesResponseApiModel<ConsultarPedidoResponseApiModel>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Inserir([FromBody] InserirPedidoRequestApiModel request)
    {
        var itens = request.Itens?.Select(item => item is null
            ? null
            : new InserirItemPedidoServiceModel(item.IdJogo, item.Quantidade)).ToList();
        var pedido = await _service.InserirAsync(itens);
        return StatusCode(StatusCodes.Status201Created, RetroGamesResponseApiModel.Sucesso(ParaResponse(pedido)));
    }

    [HttpGet("meus")]
    [ProducesResponseType<RetroGamesResponseApiModel<List<ConsultarPedidoResponseApiModel>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarMeus()
    {
        var pedidos = await _service.ListarMeusAsync();
        return Ok(RetroGamesResponseApiModel.Sucesso(pedidos.Select(ParaResponse).ToList()));
    }

    private static ConsultarPedidoResponseApiModel ParaResponse(PedidoDetalheServiceModel pedido) => new()
    {
        Id = pedido.Id,
        Data = pedido.Data,
        Situacao = pedido.Situacao,
        Total = pedido.Total,
        Itens = pedido.Itens.Select(item => new ConsultarItemPedidoResponseApiModel
        {
            IdJogo = item.IdJogo,
            NomeJogo = item.NomeJogo,
            Quantidade = item.Quantidade,
            ValorUnitario = item.ValorUnitario,
        }).ToList(),
    };
}
