using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _service;

    public GeneroController(IGeneroService service) => _service = service;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var generos = await _service.ListarAsync();
        return Ok(EnvelopeApiModel.Sucesso(generos.Select(GeneroMapper.ParaResponse).ToList()));
    }
}
