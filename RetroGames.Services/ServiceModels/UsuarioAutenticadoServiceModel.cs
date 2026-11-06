namespace RetroGames.Services.ServiceModels;

public class UsuarioAutenticadoServiceModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int IdPerfil { get; set; }
    public string DescricaoPerfil { get; set; } = string.Empty;
}
