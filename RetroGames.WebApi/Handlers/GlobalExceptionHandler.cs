using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Handlers;

/// <summary>BusinessRuleException → 400/401/403/404/409; demais exceções → 500. Sempre no formato do envelope.</summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    public const string MensagemErroPadrao = "Ocorreu um erro inesperado. Tente novamente mais tarde.";

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int status;
        EnvelopeApiModel envelope;

        if (exception is BusinessRuleException negocio)
        {
            status = ObterStatus(negocio.Erro.Tipo);
            envelope = EnvelopeApiModel.Erro(negocio.Erro.Resposta);
        }
        else
        {
            _logger.LogError(exception, "Erro não tratado.");
            status = StatusCodes.Status500InternalServerError;
            envelope = EnvelopeApiModel.Erro(ResponseCodes.ERRO, MensagemErroPadrao);
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(envelope, cancellationToken);
        return true;
    }

    public static int ObterStatus(TipoErro tipo) => tipo switch
    {
        TipoErro.Validacao => StatusCodes.Status400BadRequest,
        TipoErro.NaoAutenticado => StatusCodes.Status401Unauthorized,
        TipoErro.SemPermissao => StatusCodes.Status403Forbidden,
        TipoErro.NaoEncontrado => StatusCodes.Status404NotFound,
        TipoErro.Conflito => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError,
    };
}
