using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _service;

    public PlataformaController(IPlataformaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var plataformas = await _service.ListarAsync();
        var respostas = plataformas.Select(PlataformaMapper.ParaResponse).ToList();
        return Ok(RetroGamesResponseApiModel.Sucesso(respostas));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var plataforma = await _service.ObterAsync(id);
        return Ok(RetroGamesResponseApiModel.Sucesso(PlataformaMapper.ParaResponse(plataforma)));
    }
}
