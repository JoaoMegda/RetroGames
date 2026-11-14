namespace RetroGames.Services;

/// <summary>Expõe ao Service a identidade da requisição sem acoplá-lo ao ASP.NET Core.</summary>
public interface IUsuarioAtualService
{
    bool EstaAutenticado { get; }
    int? Id { get; }
    bool EhAdministrador { get; }
}
