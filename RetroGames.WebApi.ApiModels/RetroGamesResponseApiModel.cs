namespace RetroGames.WebApi.ApiModels;

public class RetroGamesResponseApiModel
{
    public int ResponseCode { get; set; }
    public string? ErrorMessage { get; set; }

    public static RetroGamesResponseApiModel<T> Sucesso<T>(T data) => new()
    {
        ResponseCode = ResponseCodes.SUCESSO,
        Data = data,
    };
}

public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    public T? Data { get; set; }
}
