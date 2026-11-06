using Moq;
using RetroGames.Model;
using RetroGames.Repository;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly PlataformaService _service;

    public PlataformaServiceTests()
    {
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Plataforma).Returns(_plataformas.Object);
        repositorios.SetupGet(r => r.Jogo).Returns(_jogos.Object);
        _service = new PlataformaService(repositorios.Object);
    }

    private static Plataforma NovaPlataforma(int id = 0) =>
        new() { Id = id, Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1990 };

    [Fact]
    public async Task InserirAsync_NomeJaCadastrado_LancaPlataformaNomeDuplicado()
    {
        _plataformas.Setup(r => r.ObterPorNomeAsync("Super Nintendo")).ReturnsAsync(NovaPlataforma(1));

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.InserirAsync(NovaPlataforma()));

        Assert.Equal(ResponseCodes.PLATAFORMA_NOME_DUPLICADO.Code, erro.Erro.Resposta.Code);
    }

    [Fact]
    public async Task InserirAsync_RepositorioRecusaPorUnique_LancaPlataformaNomeDuplicado()
    {
        _plataformas.Setup(r => r.ObterPorNomeAsync(It.IsAny<string>())).ReturnsAsync((Plataforma?)null);
        _plataformas.Setup(r => r.InserirAsync(It.IsAny<Plataforma>())).ThrowsAsync(new RegistroDuplicadoException());

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.InserirAsync(NovaPlataforma()));

        Assert.Equal(ResponseCodes.PLATAFORMA_NOME_DUPLICADO.Code, erro.Erro.Resposta.Code);
    }

    [Fact]
    public async Task InserirAsync_NomeNovo_RetornaPlataformaComIdGerado()
    {
        _plataformas.Setup(r => r.ObterPorNomeAsync(It.IsAny<string>())).ReturnsAsync((Plataforma?)null);
        _plataformas.Setup(r => r.InserirAsync(It.IsAny<Plataforma>())).ReturnsAsync(7);

        var resultado = await _service.InserirAsync(NovaPlataforma());

        Assert.Equal(7, resultado.Id);
    }

    [Fact]
    public async Task ExcluirAsync_ComJogosVinculados_LancaPlataformaPossuiJogos()
    {
        _plataformas.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(NovaPlataforma(1));
        _jogos.Setup(r => r.ContarPorPlataformaAsync(1)).ReturnsAsync(3);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _service.ExcluirAsync(1));

        Assert.Equal(ResponseCodes.PLATAFORMA_POSSUI_JOGOS.Code, erro.Erro.Resposta.Code);
        _plataformas.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }
}
