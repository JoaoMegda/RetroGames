using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class InserirJogoRequestValidator : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");
        RuleFor(x => x.Descricao).MaximumLength(2000).WithMessage("A descrição deve ter no máximo 2000 caracteres.");
        RuleFor(x => x.AnoLancamento).InclusiveBetween(1970, 2100).WithMessage("Ano de lançamento inválido."); // RN06
        RuleFor(x => x.Preco).GreaterThan(0).WithMessage("O preço deve ser maior que zero.");                  // RN06
        RuleFor(x => x.Estoque).GreaterThanOrEqualTo(0).WithMessage("O estoque não pode ser negativo.");       // RN06
        RuleFor(x => x.IdPlataforma).GreaterThan(0).WithMessage("Informe a plataforma.");
        RuleFor(x => x.IdCondicaoJogo).GreaterThan(0).WithMessage("Informe a condição.");
        RuleFor(x => x.IdsGenero).NotEmpty().WithMessage("Informe ao menos um gênero.");
        RuleForEach(x => x.IdsGenero).GreaterThan(0).WithMessage("Gênero inválido.");
    }
}

public class AlterarJogoRequestValidator : AbstractValidator<AlterarJogoRequestApiModel>
{
    public AlterarJogoRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");
        RuleFor(x => x.Descricao).MaximumLength(2000).WithMessage("A descrição deve ter no máximo 2000 caracteres.");
        RuleFor(x => x.AnoLancamento).InclusiveBetween(1970, 2100).WithMessage("Ano de lançamento inválido.");
        RuleFor(x => x.Preco).GreaterThan(0).WithMessage("O preço deve ser maior que zero.");
        RuleFor(x => x.Estoque).GreaterThanOrEqualTo(0).WithMessage("O estoque não pode ser negativo.");
        RuleFor(x => x.IdPlataforma).GreaterThan(0).WithMessage("Informe a plataforma.");
        RuleFor(x => x.IdCondicaoJogo).GreaterThan(0).WithMessage("Informe a condição.");
        RuleFor(x => x.IdsGenero).NotEmpty().WithMessage("Informe ao menos um gênero.");
        RuleForEach(x => x.IdsGenero).GreaterThan(0).WithMessage("Gênero inválido.");
    }
}

public class ListarJogoRequestValidator : AbstractValidator<ListarJogoRequestApiModel>
{
    public ListarJogoRequestValidator()
    {
        RuleFor(x => x.Pagina).GreaterThanOrEqualTo(1).WithMessage("Página inválida.");
        RuleFor(x => x.TamanhoPagina).InclusiveBetween(1, 50).WithMessage("O tamanho da página deve estar entre 1 e 50.");
        RuleFor(x => x.Busca).MaximumLength(100).WithMessage("A busca deve ter no máximo 100 caracteres.");
    }
}
