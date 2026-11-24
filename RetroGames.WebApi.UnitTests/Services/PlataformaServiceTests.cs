using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly PlataformaService _servico;

    public PlataformaServiceTests()
    {
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Plataforma).Returns(_plataformas.Object);
        _servico = new PlataformaService(repositorios.Object);
    }

    private static Plataforma NovaPlataforma(int id = 0) =>
        new() { Id = id, Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1990 };

    [Fact]
    public async Task ObterAsync_IdInexistente_LancaPlataformaNaoEncontrada()
    {
        // Arrange
        const int idInexistente = 42;
        _plataformas.Setup(repositorio => repositorio.ObterPorIdAsync(idInexistente))
            .ReturnsAsync((Plataforma?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.ObterAsync(idInexistente));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaNaoEncontrada, erro.Erro);
        _plataformas.Verify(repositorio => repositorio.ObterPorIdAsync(idInexistente), Times.Once);
        _plataformas.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InserirAsync_NomeJaCadastrado_LancaPlataformaNomeDuplicado()
    {
        // Arrange
        var plataforma = NovaPlataforma();
        _plataformas.Setup(r => r.ExisteComNomeAsync(plataforma.Nome, null)).ReturnsAsync(true);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync(plataforma));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaNomeDuplicado, erro.Erro);
        _plataformas.Verify(r => r.InserirAsync(It.IsAny<Plataforma>()), Times.Never);
    }

    [Fact]
    public async Task InserirAsync_RepositorioLancaRegistroDuplicado_LancaPlataformaNomeDuplicado()
    {
        // Arrange
        var plataforma = NovaPlataforma();
        _plataformas.Setup(r => r.ExisteComNomeAsync(plataforma.Nome, null)).ReturnsAsync(false);
        _plataformas.Setup(r => r.InserirAsync(plataforma)).ThrowsAsync(new RegistroDuplicadoException());

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync(plataforma));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaNomeDuplicado, erro.Erro);
        _plataformas.Verify(r => r.InserirAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task InserirAsync_NomeNovo_RetornaPlataformaComIdGerado()
    {
        // Arrange
        const int idGerado = 7;
        var plataforma = NovaPlataforma();
        _plataformas.Setup(r => r.ExisteComNomeAsync(plataforma.Nome, null)).ReturnsAsync(false);
        _plataformas.Setup(r => r.InserirAsync(plataforma)).ReturnsAsync(idGerado);

        // Act
        var resultado = await _servico.InserirAsync(plataforma);

        // Assert
        Assert.Equal(idGerado, resultado.Id);
        _plataformas.Verify(r => r.InserirAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task AlterarAsync_MantendoProprioNome_GravaAlteracao()
    {
        // Arrange
        const int id = 7;
        const string fabricanteNovo = "Nintendo do Brasil";
        var cadastrada = NovaPlataforma(id);
        var alteracao = NovaPlataforma(id);
        alteracao.Fabricante = fabricanteNovo;

        _plataformas.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(cadastrada);
        _plataformas.Setup(r => r.ExisteComNomeAsync(cadastrada.Nome, null)).ReturnsAsync(true);
        _plataformas.Setup(r => r.ExisteComNomeAsync(cadastrada.Nome, id)).ReturnsAsync(false);
        _plataformas.Setup(r => r.AlterarAsync(alteracao)).Returns(Task.CompletedTask);

        // Act
        var resultado = await _servico.AlterarAsync(alteracao);

        // Assert
        Assert.Equal(cadastrada.Nome, resultado.Nome);
        Assert.Equal(fabricanteNovo, resultado.Fabricante);
        _plataformas.Verify(r => r.ExisteComNomeAsync(cadastrada.Nome, id), Times.Once);
        _plataformas.Verify(r => r.AlterarAsync(It.Is<Plataforma>(p =>
            p.Id == id && p.Nome == cadastrada.Nome && p.Fabricante == fabricanteNovo)), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_ComJogosVinculados_LancaPlataformaPossuiJogos()
    {
        // Arrange
        const int id = 1;
        _plataformas.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(NovaPlataforma(id));
        _plataformas.Setup(r => r.PossuiJogosAsync(id)).ReturnsAsync(true);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.ExcluirAsync(id));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaPossuiJogos, erro.Erro);
        _plataformas.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }
}
