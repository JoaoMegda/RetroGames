using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuracao;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _service;

    public JogoController(IJogoService service) => _service = service;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar([FromQuery] ListarJogoRequestApiModel request)
    {
        var lista = await _service.ListarAsync(JogoMapper.ParaServiceModel(request));
        return Ok(EnvelopeApiModel.Sucesso(JogoMapper.ParaPaginaResponse(lista)));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var jogo = await _service.ObterAsync(id);
        return Ok(EnvelopeApiModel.Sucesso(JogoMapper.ParaDetalheResponse(jogo)));
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Inserir([FromBody] InserirJogoRequestApiModel request)
    {
        var jogo = await _service.InserirAsync(JogoMapper.ParaModel(request), request.IdsGenero);
        var resposta = EnvelopeApiModel.Sucesso(JogoMapper.ParaDetalheResponse(jogo));
        return CreatedAtAction(nameof(ListarPorId), new { id = jogo.Id }, resposta);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Alterar(int id, [FromBody] AlterarJogoRequestApiModel request)
    {
        var jogo = await _service.AlterarAsync(
            JogoMapper.ParaModel(id, request), request.IdsGenero, request.ManterEstoque);
        return Ok(EnvelopeApiModel.Sucesso(JogoMapper.ParaDetalheResponse(jogo)));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _service.DesativarAsync(id);
        return Ok(EnvelopeApiModel.Sucesso());
    }
}
