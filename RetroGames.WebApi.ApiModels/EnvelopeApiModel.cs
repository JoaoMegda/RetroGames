using System.Text.Json.Serialization;

namespace RetroGames.WebApi.ApiModels;

/// <summary>Envelope de toda resposta: { responseCode, data, errorMessage }. "data" é omitido quando nulo.</summary>
public class EnvelopeApiModel
{
    public int ResponseCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; set; }

    public string? ErrorMessage { get; set; }

    public static EnvelopeApiModel Sucesso(object? data = null) =>
        new() { ResponseCode = ResponseCodes.SUCESSO, Data = data };

    public static EnvelopeApiModel Erro((int Code, string DefaultMessage) resposta, object? data = null) =>
        new() { ResponseCode = resposta.Code, ErrorMessage = resposta.DefaultMessage, Data = data };

    public static EnvelopeApiModel Erro(int code, string mensagem) =>
        new() { ResponseCode = code, ErrorMessage = mensagem };
}
