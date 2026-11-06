namespace RetroGames.WebApi.ApiModels;

public class ConsultarGeneroResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class ConsultarCondicaoJogoResponseApiModel
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
