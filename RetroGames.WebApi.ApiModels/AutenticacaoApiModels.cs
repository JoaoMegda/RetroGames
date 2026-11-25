namespace RetroGames.WebApi.ApiModels;

public class LoginRequestApiModel
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class CadastrarUsuarioRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class UsuarioLogadoResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int IdPerfil { get; set; }
    public string DescricaoPerfil { get; set; } = string.Empty;
}
