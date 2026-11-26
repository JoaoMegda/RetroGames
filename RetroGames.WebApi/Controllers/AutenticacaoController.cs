using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _service;
    public AutenticacaoController(IAutenticacaoService service) => _service = service;

    [HttpGet("csrf")]
    [AllowAnonymous]
    public IActionResult ObterCsrf([FromServices] IAntiforgery antiforgery)
    {
        var token = antiforgery.GetAndStoreTokens(HttpContext).RequestToken!;
        Response.Cookies.Append("XSRF-TOKEN", token, new CookieOptions
        {
            HttpOnly = false,
            SameSite = SameSiteMode.Strict,
            Secure = Request.IsHttps,
            Path = "/",
        });

        return Ok(EnvelopeApiModel.Sucesso(token));
    }

    [HttpPost("cadastrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Cadastrar([FromBody] CadastrarUsuarioRequestApiModel request)
    {
        var usuario = await _service.CadastrarAsync(request.Nome, request.Email, request.Senha);
        await IniciarSessaoAsync(usuario);

        return StatusCode(StatusCodes.Status201Created, EnvelopeApiModel.Sucesso(ParaResponse(usuario)));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestApiModel request)
    {
        var usuario = await _service.AutenticarAsync(request.Email.Trim(), request.Senha);

        if (usuario is null)
            return Unauthorized(EnvelopeApiModel.Erro(ResponseCodes.CREDENCIAIS_INVALIDAS));

        await IniciarSessaoAsync(usuario);

        return Ok(EnvelopeApiModel.Sucesso(ParaResponse(usuario)));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(EnvelopeApiModel.Sucesso());
    }

    [HttpGet("eu")]
    public async Task<IActionResult> Eu()
    {
        if (User.ObterIdUsuario() is not int id)
            return Unauthorized(EnvelopeApiModel.Erro(ResponseCodes.NAO_AUTENTICADO));

        var usuario = await _service.ObterUsuarioAsync(id);
        return Ok(EnvelopeApiModel.Sucesso(ParaResponse(usuario)));
    }

    private Task IniciarSessaoAsync(UsuarioAutenticadoServiceModel usuario) =>
        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usuario.ParaClaimsPrincipal());

    private static UsuarioLogadoResponseApiModel ParaResponse(UsuarioAutenticadoServiceModel usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        IdPerfil = usuario.IdPerfil,
        DescricaoPerfil = usuario.DescricaoPerfil,
    };
}
