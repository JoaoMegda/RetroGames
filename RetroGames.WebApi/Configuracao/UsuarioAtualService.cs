using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Services;

namespace RetroGames.WebApi.Configuracao;

/// <summary>Lê a identidade autenticada da requisição HTTP atual.</summary>
public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _acessor;

    public UsuarioAtualService(IHttpContextAccessor acessor) => _acessor = acessor;

    private ClaimsPrincipal? Usuario => _acessor.HttpContext?.User;

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated == true;

    public int? Id => int.TryParse(Usuario?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
        ? id
        : null;

    public bool EhAdministrador => Usuario?.IsInRole(Perfis.NomeAdministrador) == true;
}
