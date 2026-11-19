using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuracao;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _service;

    public PlataformaController(IPlataformaService service) => _service = service;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var plataformas = await _service.ListarAsync();
        var respostas = plataformas.Select(PlataformaMapper.ParaResponse).ToList();
        return Ok(RetroGamesResponseApiModel.Sucesso(respostas));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var plataforma = await _service.ObterAsync(id);
        return Ok(RetroGamesResponseApiModel.Sucesso(PlataformaMapper.ParaResponse(plataforma)));
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Inserir([FromBody] InserirPlataformaRequestApiModel request)
    {
        var plataforma = await _service.InserirAsync(PlataformaMapper.ParaModel(request));
        var resposta = RetroGamesResponseApiModel.Sucesso(PlataformaMapper.ParaResponse(plataforma));
        return CreatedAtAction(nameof(ListarPorId), new { id = plataforma.Id }, resposta);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Alterar(int id, [FromBody] AlterarPlataformaRequestApiModel request)
    {
        var plataforma = await _service.AlterarAsync(PlataformaMapper.ParaModel(id, request));
        return Ok(RetroGamesResponseApiModel.Sucesso(PlataformaMapper.ParaResponse(plataforma)));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Excluir(int id)
    {
        await _service.ExcluirAsync(id);
        return Ok(RetroGamesResponseApiModel.Sucesso<object?>(null));
    }
}
