using RetroGames.WebApi.ApiModels;

namespace RetroGames.Services;

/// <summary>Categoria do erro. O GlobalExceptionHandler converte em status HTTP; o Service não conhece HTTP.</summary>
public enum TipoErro { Validacao, NaoAutenticado, SemPermissao, NaoEncontrado, Conflito }

public readonly record struct BusinessError((int Code, string DefaultMessage) Resposta, TipoErro Tipo);

public static class BusinessErrors
{
    public static readonly BusinessError NaoAutenticado = new(ResponseCodes.NAO_AUTENTICADO, TipoErro.NaoAutenticado);
    public static readonly BusinessError CredenciaisInvalidas = new(ResponseCodes.CREDENCIAIS_INVALIDAS, TipoErro.NaoAutenticado);
    public static readonly BusinessError PlataformaNaoEncontrada = new(ResponseCodes.PLATAFORMA_NAO_ENCONTRADA, TipoErro.NaoEncontrado);
    public static readonly BusinessError PlataformaNomeDuplicado = new(ResponseCodes.PLATAFORMA_NOME_DUPLICADO, TipoErro.Conflito);
    public static readonly BusinessError PlataformaPossuiJogos = new(ResponseCodes.PLATAFORMA_POSSUI_JOGOS, TipoErro.Conflito);
    public static readonly BusinessError JogoNaoEncontrado = new(ResponseCodes.JOGO_NAO_ENCONTRADO, TipoErro.NaoEncontrado);
    public static readonly BusinessError PlataformaInexistente = new(ResponseCodes.PLATAFORMA_INEXISTENTE, TipoErro.Validacao);
    public static readonly BusinessError CondicaoInexistente = new(ResponseCodes.CONDICAO_INEXISTENTE, TipoErro.Validacao);
    public static readonly BusinessError GeneroInexistente = new(ResponseCodes.GENERO_INEXISTENTE, TipoErro.Validacao);
}
