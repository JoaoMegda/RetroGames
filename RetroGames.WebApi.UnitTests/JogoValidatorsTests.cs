using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class JogoValidatorsTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void InserirEAlterarJogo_ValidamPrecoEmCentavos(int centavos, bool valido)
    {
        var inserir = NovoRequestInserir();
        var alterar = NovoRequestAlterar();
        var preco = centavos / 100m;
        inserir.Preco = preco;
        alterar.Preco = preco;

        Assert.Equal(valido, new InserirJogoRequestValidator().Validate(inserir).IsValid);
        Assert.Equal(valido, new AlterarJogoRequestValidator().Validate(alterar).IsValid);
    }

    [Fact]
    public void InserirJogo_DescricaoAcimaDeMilCaracteres_EInvalida()
    {
        var request = NovoRequestInserir();
        request.Descricao = new string('x', 1001);

        var resultado = new InserirJogoRequestValidator().Validate(request);

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(request.Descricao));
    }

    [Fact]
    public void AlterarJogo_IncluiAtivoNoModeloMapeado()
    {
        var propriedadeAtivo = typeof(AlterarJogoRequestApiModel).GetProperty("Ativo");
        Assert.NotNull(propriedadeAtivo);

        var request = NovoRequestAlterar();
        propriedadeAtivo!.SetValue(request, false);
        var jogo = JogoMapper.ParaModel(42, request);

        Assert.False(jogo.Ativo);
    }

    private static InserirJogoRequestApiModel NovoRequestInserir() => new()
    {
        Nome = "Jogo teste",
        AnoLancamento = 1990,
        Preco = 0.01m,
        Estoque = 0,
        IdPlataforma = 1,
        IdCondicaoJogo = 1,
        IdsGenero = new List<int> { 1 },
    };

    private static AlterarJogoRequestApiModel NovoRequestAlterar() => new()
    {
        Nome = "Jogo teste",
        AnoLancamento = 1990,
        Preco = 0.01m,
        Estoque = 0,
        IdPlataforma = 1,
        IdCondicaoJogo = 1,
        IdsGenero = new List<int> { 1 },
    };
}
