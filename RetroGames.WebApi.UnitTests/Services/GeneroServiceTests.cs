using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    [Fact]
    public async Task ListarAsync_RepositorioComGeneros_DevolveGenerosRecebidos()
    {
        // Arrange
        var generos = new List<Genero>
        {
            new() { Id = 7, Nome = "Aventura" },
            new() { Id = 3, Nome = "Corrida" },
        };
        var generosEsperados = generos.Select(g => (g.Id, g.Nome)).ToArray();
        var repositorio = new Mock<IGeneroRepository>();
        repositorio.Setup(r => r.ListarAsync()).ReturnsAsync(generos);
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Genero).Returns(repositorio.Object);
        var servico = new GeneroService(repositorios.Object);

        // Act
        var resultado = await servico.ListarAsync();

        // Assert
        Assert.Equal(generosEsperados, resultado.Select(g => (g.Id, g.Nome)));
        repositorio.Verify(r => r.ListarAsync(), Times.Once);
    }
}
