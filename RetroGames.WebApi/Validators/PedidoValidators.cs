using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class InserirPedidoRequestValidator : AbstractValidator<InserirPedidoRequestApiModel>
{
    public InserirPedidoRequestValidator()
    {
        RuleFor(x => x.Itens).NotEmpty().WithMessage("Informe ao menos um item.");
        RuleForEach(x => x.Itens)
            .NotNull().WithMessage("Informe um item válido.")
            .ChildRules(item =>
            {
                item.RuleFor(x => x!.IdJogo).GreaterThan(0).WithMessage("Informe o jogo.");
                item.RuleFor(x => x!.Quantidade).InclusiveBetween(1, 10)
                    .WithMessage("Informe uma quantidade entre 1 e 10.");
            });
    }
}
