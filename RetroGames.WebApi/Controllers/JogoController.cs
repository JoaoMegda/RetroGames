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

    [HttpPost]
    public async Task<IActionResult> Inserir([FromBody] InserirJogoRequestApiModel request)
    {
        var jogo = await _service.InserirAsync(JogoMapper.ParaModel(request), request.IdsGenero);
        var resposta = EnvelopeApiModel.Sucesso(JogoMapper.ParaDetalheResponse(jogo));
        return CreatedAtAction(nameof(ListarPorId), new { id = jogo.Id }, resposta);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Alterar(int id, [FromBody] AlterarJogoRequestApiModel request)
    {
        var jogo = await _service.AlterarAsync(JogoMapper.ParaModel(id, request), request.IdsGenero);
        return Ok(EnvelopeApiModel.Sucesso(JogoMapper.ParaDetalheResponse(jogo)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Desativar(int id)
    {
        await _service.DesativarAsync(id);
        return Ok(EnvelopeApiModel.Sucesso());
    }
}
