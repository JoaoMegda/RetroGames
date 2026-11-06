using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _service;

    public GeneroController(IGeneroService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var generos = await _service.ListarAsync();
        return Ok(EnvelopeApiModel.Sucesso(generos
            .Select(g => new ConsultarGeneroResponseApiModel { Id = g.Id, Nome = g.Nome }).ToList()));
    }
}
