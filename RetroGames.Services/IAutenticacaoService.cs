using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public interface IAutenticacaoService
{
    Task<UsuarioAutenticadoServiceModel> AutenticarAsync(string email, string senha);
    Task<UsuarioAutenticadoServiceModel> ObterUsuarioAsync(int id);
    Task GarantirAdministradorInicialAsync(string email, string senha);
}
