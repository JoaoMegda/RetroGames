using Microsoft.AspNetCore.Authorization;
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
        return Ok(EnvelopeApiModel.Sucesso(plataformas.Select(PlataformaMapper.ParaResponse).ToList()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Consultar(int id)
    {
        var plataforma = await _service.ObterAsync(id);
        return Ok(EnvelopeApiModel.Sucesso(PlataformaMapper.ParaResponse(plataforma)));
    }

    [HttpPost]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Inserir([FromBody] InserirPlataformaRequestApiModel request)
    {
        var plataforma = await _service.InserirAsync(PlataformaMapper.ParaModel(request));
        return StatusCode(StatusCodes.Status201Created, EnvelopeApiModel.Sucesso(PlataformaMapper.ParaResponse(plataforma)));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Alterar(int id, [FromBody] AlterarPlataformaRequestApiModel request)
    {
        var plataforma = await _service.AlterarAsync(PlataformaMapper.ParaModel(id, request));
        return Ok(EnvelopeApiModel.Sucesso(PlataformaMapper.ParaResponse(plataforma)));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _service.ExcluirAsync(id);
        return Ok(EnvelopeApiModel.Sucesso());
    }
}
