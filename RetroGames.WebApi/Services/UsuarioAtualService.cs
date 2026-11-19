using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

/// <summary>Lê a identidade autenticada da requisição HTTP atual.</summary>
public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _acessor;

    public UsuarioAtualService(IHttpContextAccessor acessor) => _acessor = acessor;

    private ClaimsPrincipal? Usuario => _acessor.HttpContext?.User;

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated == true;

    public int? Id => Usuario?.ObterIdUsuario();

    public bool EhAdministrador => Usuario?.IsInRole(Perfis.NomeAdministrador) == true;
}
