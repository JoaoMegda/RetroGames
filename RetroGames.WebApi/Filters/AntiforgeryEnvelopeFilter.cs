using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

public class AntiforgeryEnvelopeFilter : IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext contexto)
    {
        if (contexto.Result is IAntiforgeryValidationFailedResult)
        {
            var erros = new { csrf = new[] { "Token CSRF ausente ou inválido." } };
            contexto.Result = new BadRequestObjectResult(
                EnvelopeApiModel.Erro(ResponseCodes.MODEL_STATE_INVALIDO, erros));
        }
    }

    public void OnResultExecuted(ResultExecutedContext contexto) { }
}
