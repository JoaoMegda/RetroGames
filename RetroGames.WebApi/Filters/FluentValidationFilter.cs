using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

/// <summary>Valida os argumentos da action com os validators registrados e responde no formato do envelope.</summary>
public class FluentValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _servicos;
    private readonly IOptions<ApiBehaviorOptions> _opcoesApi;

    public FluentValidationFilter(IServiceProvider servicos, IOptions<ApiBehaviorOptions> opcoesApi)
    {
        _servicos = servicos;
        _opcoesApi = opcoesApi;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var possuiParametroBody = context.ActionDescriptor.Parameters
            .Any(parametro => parametro.BindingInfo?.BindingSource == BindingSource.Body);

        // Corpo realmente ausente mantém o código específico MODEL_NULL, mesmo quando o binder o registra no ModelState.
        if (possuiParametroBody && context.HttpContext.Request.ContentLength is null or 0 &&
            context.ActionDescriptor.Parameters.Any(parametro =>
                parametro.BindingInfo?.BindingSource == BindingSource.Body &&
                (!context.ActionArguments.TryGetValue(parametro.Name, out var valor) || valor is null)))
        {
            context.Result = new ObjectResult(EnvelopeApiModel.Erro(ResponseCodes.MODEL_NULL))
            { StatusCode = StatusCodes.Status400BadRequest };
            return;
        }

        // Erros de binding (por exemplo, pagina=abc ou JSON inválido) usam o envelope genérico do código 3.
        if (!context.ModelState.IsValid)
        {
            context.Result = _opcoesApi.Value.InvalidModelStateResponseFactory(context);
            return;
        }

        // Um corpo ausente ou JSON null é rejeitado antes de chegar ao Service.
        foreach (var parametro in context.ActionDescriptor.Parameters)
        {
            if (parametro.BindingInfo?.BindingSource != BindingSource.Body) continue;
            if (!context.ActionArguments.TryGetValue(parametro.Name, out var valor) || valor is null)
            {
                context.Result = new ObjectResult(EnvelopeApiModel.Erro(ResponseCodes.MODEL_NULL))
                { StatusCode = StatusCodes.Status400BadRequest };
                return;
            }
        }

        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento is null) continue;

            var tipoValidador = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (_servicos.GetService(tipoValidador) is not IValidator validador) continue;

            var resultado = await validador.ValidateAsync(new ValidationContext<object>(argumento));
            if (resultado.IsValid) continue;

            var erros = resultado.Errors
                .GroupBy(e => JsonNamingPolicy.CamelCase.ConvertName(e.PropertyName))
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

            context.Result = new ObjectResult(EnvelopeApiModel.Erro(ResponseCodes.MODEL_STATE_INVALIDO, erros))
            { StatusCode = StatusCodes.Status400BadRequest };
            return;
        }

        await next();
    }
}
