using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Controllers;

/// <summary>Uma classe por tabela (AGENTS.md), mas a rota segue o contrato da especificação (5.2).</summary>
[ApiController]
[Route("api/TabelaTipo/condicoes-jogo")]
public class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService _service;

    public CondicaoJogoController(ICondicaoJogoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var condicoes = await _service.ListarAsync();
        return Ok(EnvelopeApiModel.Sucesso(condicoes
            .Select(c => new ConsultarCondicaoJogoResponseApiModel { Id = c.Id, Descricao = c.Descricao }).ToList()));
    }
}
