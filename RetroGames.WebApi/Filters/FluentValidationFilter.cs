using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

/// <summary>Valida os argumentos da action com os validators registrados e responde no formato do envelope.</summary>
public class FluentValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _servicos;

    public FluentValidationFilter(IServiceProvider servicos) => _servicos = servicos;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Corpo ausente ou JSON ilegível: o binder entrega null.
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
