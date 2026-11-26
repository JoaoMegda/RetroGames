using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class AutenticacaoValidatorsTests
{
    private readonly CadastrarUsuarioRequestValidator _validator = new();

    [Fact]
    public void CadastrarUsuario_ValoresValidados_AceitaModelo()
    {
        var request = new CadastrarUsuarioRequestApiModel
        {
            Nome = new string('a', 100),
            Email = new string('a', 55) + "@" + new string('b', 60) + "." + new string('c', 28) + ".test",
            Senha = new string('c', 8),
        };

        Assert.True(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void CadastrarUsuario_ValoresForaDosLimites_IdentificaCamposInvalidos()
    {
        var request = new CadastrarUsuarioRequestApiModel
        {
            Nome = new string('a', 101),
            Email = new string('a', 55) + "@" + new string('b', 60) + "." + new string('c', 29) + ".test",
            Senha = new string('c', 129),
        };

        var resultado = _validator.Validate(request);

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == "Nome");
        Assert.Contains(resultado.Errors, erro => erro.PropertyName == "Email");
        Assert.Contains(resultado.Errors, erro => erro.PropertyName == "Senha");
    }

    [Theory]
    [InlineData("invalido")]
    [InlineData("cliente@ ")]
    [InlineData("Cliente <cliente@example.test>")]
    public void CadastrarUsuario_NomeEmBrancoEmailMalformadoESenhaCurta_IdentificaCamposInvalidos(string email)
    {
        var request = new CadastrarUsuarioRequestApiModel
        {
            Nome = " ",
            Email = email,
            Senha = new string('c', 7),
        };

        var resultado = _validator.Validate(request);

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == "Nome");
        Assert.Contains(resultado.Errors, erro => erro.PropertyName == "Email");
        Assert.Contains(resultado.Errors, erro => erro.PropertyName == "Senha");
    }
}
