using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task ListarAsync_RepositorioComCondicoes_DevolveCondicoesRecebidas()
    {
        // Arrange
        var condicoes = new List<CondicaoJogo>
        {
            new() { Id = 4, Descricao = "Lacrado" },
            new() { Id = 2, Descricao = "Usado" },
        };
        var condicoesEsperadas = condicoes.Select(c => (c.Id, c.Descricao)).ToArray();
        var repositorio = new Mock<ICondicaoJogoRepository>();
        repositorio.Setup(r => r.ListarAsync()).ReturnsAsync(condicoes);
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.CondicaoJogo).Returns(repositorio.Object);
        var servico = new CondicaoJogoService(repositorios.Object);

        // Act
        var resultado = await servico.ListarAsync();

        // Assert
        Assert.Equal(condicoesEsperadas, resultado.Select(c => (c.Id, c.Descricao)));
        repositorio.Verify(r => r.ListarAsync(), Times.Once);
    }
}
