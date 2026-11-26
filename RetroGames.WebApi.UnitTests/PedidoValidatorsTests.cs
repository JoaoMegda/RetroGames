using System.Text.Json;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class PedidoValidatorsTests
{
    [Theory]
    [InlineData("{}")]
    [InlineData("""{"itens":null}""")]
    [InlineData("""{"itens":[]}""")]
    [InlineData("""{"itens":[null]}""")]
    [InlineData("""{"itens":[{"idJogo":1,"quantidade":0}]}""")]
    [InlineData("""{"itens":[{"idJogo":1,"quantidade":11}]}""")]
    [InlineData("""{"itens":[{"idJogo":0,"quantidade":1}]}""")]
    public void InserirPedido_ComItensInvalidos_RejeitaRequisicao(string json)
    {
        var opcoes = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var request = JsonSerializer.Deserialize<InserirPedidoRequestApiModel>(json, opcoes)!;

        Assert.False(new InserirPedidoRequestValidator().Validate(request).IsValid);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void InserirPedido_ComQuantidadeNoLimite_AceitaRequisicao(int quantidade)
    {
        var request = new InserirPedidoRequestApiModel
        {
            Itens = [new() { IdJogo = 1, Quantidade = quantidade }],
        };

        Assert.True(new InserirPedidoRequestValidator().Validate(request).IsValid);
    }
}
