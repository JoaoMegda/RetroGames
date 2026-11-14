using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _service;

    public JogoController(IJogoService service) => _service = service;

    private bool EhAdministrador => User.IsInRole(Perfis.NomeAdministrador);

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] ListarJogoRequestApiModel request)
    {
        var lista = await _service.ListarAsync(JogoMapper.ParaServiceModel(request), ehAdministrador: false);
        return Ok(EnvelopeApiModel.Sucesso(JogoMapper.ParaPaginaResponse(lista)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var jogo = await _service.ObterAsync(id, EhAdministrador);
        return Ok(EnvelopeApiModel.Sucesso(JogoMapper.ParaDetalheResponse(jogo)));
    }
}
