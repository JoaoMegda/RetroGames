using System.Net.Mail;
using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestApiModel>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Informe o e-mail.")
            .EmailAddress().WithMessage("E-mail inválido.");
        RuleFor(x => x.Senha).NotEmpty().WithMessage("Informe a senha.");
    }
}

public class CadastrarUsuarioRequestValidator : AbstractValidator<CadastrarUsuarioRequestApiModel>
{
    public CadastrarUsuarioRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(100).WithMessage("O nome deve ter até 100 caracteres.");
        RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Informe o e-mail.")
            .MaximumLength(150).WithMessage("O e-mail deve ter até 150 caracteres.")
            .Must(EmailValido).WithMessage("E-mail inválido.");
        RuleFor(x => x.Senha).NotEmpty().WithMessage("Informe a senha.")
            .Length(8, 128).WithMessage("A senha deve ter entre 8 e 128 caracteres.");
    }

    private static bool EmailValido(string email)
    {
        var emailNormalizado = email.Trim();
        return MailAddress.TryCreate(emailNormalizado, out var endereco)
            && endereco.Address == emailNormalizado;
    }
}
