using System.Net;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.Services;

public readonly record struct BusinessError((int Code, string DefaultMessage) Resposta, HttpStatusCode Status);

public static class BusinessErrors
{
    public static readonly BusinessError NaoAutenticado = new(ResponseCodes.NAO_AUTENTICADO, HttpStatusCode.Unauthorized);

    public static readonly BusinessError PlataformaNaoEncontrada = new((100, "Plataforma não encontrada."), HttpStatusCode.NotFound);
    public static readonly BusinessError PlataformaNomeDuplicado = new((101, "Já existe uma plataforma com este nome."), HttpStatusCode.Conflict);
    public static readonly BusinessError PlataformaPossuiJogos = new((102, "Não é possível excluir: existem jogos desta plataforma."), HttpStatusCode.Conflict);
    public static readonly BusinessError JogoNaoEncontrado = new((103, "Jogo não encontrado."), HttpStatusCode.NotFound);
    public static readonly BusinessError CondicaoNaoEncontrada = new((104, "Condição do jogo não encontrada."), HttpStatusCode.NotFound);
    public static readonly BusinessError GeneroNaoEncontrado = new((105, "Gênero não encontrado."), HttpStatusCode.NotFound);

    public static readonly BusinessError PlataformaInexistente = new(ResponseCodes.PLATAFORMA_INEXISTENTE, HttpStatusCode.BadRequest);
    public static readonly BusinessError CondicaoInexistente = new(ResponseCodes.CONDICAO_INEXISTENTE, HttpStatusCode.BadRequest);
    public static readonly BusinessError GeneroInexistente = new(ResponseCodes.GENERO_INEXISTENTE, HttpStatusCode.BadRequest);
}
