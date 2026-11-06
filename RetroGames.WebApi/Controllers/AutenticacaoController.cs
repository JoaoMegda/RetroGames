using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _service;
    private readonly IAntiforgery _antiforgery;

    public AutenticacaoController(IAutenticacaoService service, IAntiforgery antiforgery)
    {
        _service = service;
        _antiforgery = antiforgery;
    }

    /// <summary>Emite o token no cookie XSRF-TOKEN (legível pelo JS). Chamar de novo após login e logout.</summary>
    [HttpGet("csrf")]
    public IActionResult Csrf()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
        {
            HttpOnly = false,
            SameSite = SameSiteMode.Strict,
            Secure = Request.IsHttps,
        });
        return Ok(EnvelopeApiModel.Sucesso());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AutenticarUsuarioRequestApiModel request)
    {
        var usuario = await _service.AutenticarAsync(request.Email.Trim(), request.Senha);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.DescricaoPerfil),
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Ok(EnvelopeApiModel.Sucesso(ParaResponse(usuario)));
    }

    [HttpPost("logout")]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(EnvelopeApiModel.Sucesso());
    }

    [HttpGet("eu")]
    [Authorize(Policy = "SomenteAdministrador")]
    public async Task<IActionResult> Eu()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await _service.ObterUsuarioAsync(id);
        return Ok(EnvelopeApiModel.Sucesso(ParaResponse(usuario)));
    }

    private static ConsultarUsuarioResponseApiModel ParaResponse(UsuarioAutenticadoServiceModel usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        IdPerfil = usuario.IdPerfil,
        DescricaoPerfil = usuario.DescricaoPerfil,
    };
}
