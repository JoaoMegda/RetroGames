using Moq;
using RetroGames.Model;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using RetroGames.Services.ServiceModels;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<ICondicaoJogoRepository> _condicoes = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IJogoGeneroRepository> _jogosGeneros = new();
    private readonly JogoService _servico;

    public JogoServiceTests()
    {
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Jogo).Returns(_jogos.Object);
        repositorios.SetupGet(r => r.Plataforma).Returns(_plataformas.Object);
        repositorios.SetupGet(r => r.CondicaoJogo).Returns(_condicoes.Object);
        repositorios.SetupGet(r => r.Genero).Returns(_generos.Object);
        repositorios.SetupGet(r => r.JogoGenero).Returns(_jogosGeneros.Object);
        _servico = new JogoService(repositorios.Object, Mock.Of<IUsuarioAtualService>());
    }

    [Fact]
    public async Task InserirAsync_PlataformaInexistente_LancaPlataformaNaoEncontrada()
    {
        // Arrange
        var jogo = NovoJogo();
        _plataformas.Setup(r => r.ObterPorIdAsync(jogo.IdPlataforma)).ReturnsAsync((Plataforma?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync(jogo, [3]));

        // Assert
        Assert.Equal(BusinessErrors.PlataformaNaoEncontrada, erro.Erro);
        VerificarQueNaoHouveInsercao();
    }

    [Fact]
    public async Task InserirAsync_CondicaoInexistente_LancaCondicaoNaoEncontrada()
    {
        // Arrange
        var jogo = NovoJogo();
        _plataformas.Setup(r => r.ObterPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(new Plataforma { Id = jogo.IdPlataforma });
        _condicoes.Setup(r => r.ObterPorIdAsync(jogo.IdCondicaoJogo)).ReturnsAsync((CondicaoJogo?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync(jogo, [3]));

        // Assert
        Assert.Equal(BusinessErrors.CondicaoNaoEncontrada, erro.Erro);
        VerificarQueNaoHouveInsercao();
    }

    [Fact]
    public async Task InserirAsync_GeneroInexistente_LancaGeneroNaoEncontrado()
    {
        // Arrange
        var jogo = NovoJogo();
        const int idGeneroExistente = 3;
        const int idGeneroInexistente = 99;
        _plataformas.Setup(r => r.ObterPorIdAsync(jogo.IdPlataforma))
            .ReturnsAsync(new Plataforma { Id = jogo.IdPlataforma });
        _condicoes.Setup(r => r.ObterPorIdAsync(jogo.IdCondicaoJogo))
            .ReturnsAsync(new CondicaoJogo { Id = jogo.IdCondicaoJogo });
        _generos.Setup(r => r.ListarPorIdsAsync(It.IsAny<IReadOnlyCollection<int>>()))
            .ReturnsAsync([new Genero { Id = idGeneroExistente }]);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            _servico.InserirAsync(jogo, [idGeneroExistente, idGeneroInexistente]));

        // Assert
        Assert.Equal(BusinessErrors.GeneroNaoEncontrado, erro.Erro);
        VerificarQueNaoHouveInsercao();
    }

    [Fact]
    public async Task ListarAsync_ComGenero_RestringeAosIdsDosJogosDoGenero()
    {
        // Arrange
        const int idGenero = 6;
        var idsEsperados = new[] { 7, 12 };
        _jogosGeneros.Setup(r => r.ListarPorGeneroAsync(idGenero)).ReturnsAsync([
            new JogoGenero { IdJogo = 12, IdGenero = idGenero },
            new JogoGenero { IdJogo = 7, IdGenero = idGenero },
        ]);
        _jogos.Setup(r => r.ContarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync(0);
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync([]);

        // Act
        await _servico.ListarAsync(new JogoFiltroServiceModel { IdGenero = idGenero, Busca = "Sem resultado" });

        // Assert
        _jogosGeneros.Verify(r => r.ListarPorGeneroAsync(idGenero), Times.Once);
        _jogos.Verify(r => r.ContarAsync(It.Is<JogoFiltro>(f =>
            f.IdsJogo != null && f.IdsJogo.OrderBy(id => id).SequenceEqual(idsEsperados))), Times.Once);
        _jogos.Verify(r => r.ListarAsync(It.Is<JogoFiltro>(f =>
            f.IdsJogo != null && f.IdsJogo.OrderBy(id => id).SequenceEqual(idsEsperados))), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_SemGenero_NaoConsultaVinculosParaFiltrarNemRestringeIds()
    {
        // Arrange
        _jogos.Setup(r => r.ContarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync(0);
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync([]);

        // Act
        var resultado = await _servico.ListarAsync(new JogoFiltroServiceModel());

        // Assert
        Assert.Empty(resultado.Itens);
        _jogosGeneros.VerifyNoOtherCalls();
        _jogos.Verify(r => r.ContarAsync(It.Is<JogoFiltro>(f => f.IdsJogo == null)), Times.Once);
        _jogos.Verify(r => r.ListarAsync(It.Is<JogoFiltro>(f => f.IdsJogo == null)), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_PaginaComJogos_MontaPlataformaECondicaoDeCadaJogo()
    {
        // Arrange
        const int pagina = 2;
        const int tamanhoPagina = 2;
        var jogo1 = new Jogo { Id = 7, IdPlataforma = 1, IdCondicaoJogo = 2 };
        var jogo2 = new Jogo { Id = 12, IdPlataforma = 3, IdCondicaoJogo = 4 };
        _jogos.Setup(r => r.ContarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync(5);
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync([jogo1, jogo2]);
        _jogosGeneros.Setup(r => r.ListarPorJogosAsync(It.IsAny<IReadOnlyCollection<int>>())).ReturnsAsync([]);
        _generos.Setup(r => r.ListarAsync()).ReturnsAsync([]);
        _plataformas.Setup(r => r.ListarAsync()).ReturnsAsync([
            new Plataforma { Id = jogo2.IdPlataforma, Nome = "Mega Drive" },
            new Plataforma { Id = jogo1.IdPlataforma, Nome = "Super Nintendo" },
        ]);
        _condicoes.Setup(r => r.ListarAsync()).ReturnsAsync([
            new CondicaoJogo { Id = jogo2.IdCondicaoJogo, Descricao = "Lacrado" },
            new CondicaoJogo { Id = jogo1.IdCondicaoJogo, Descricao = "Usado" },
        ]);

        // Act
        var resultado = await _servico.ListarAsync(new JogoFiltroServiceModel
        {
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
        });

        // Assert
        Assert.Collection(resultado.Itens,
            detalhe =>
            {
                Assert.Equal(jogo1.Id, detalhe.Id);
                Assert.Equal("Super Nintendo", detalhe.NomePlataforma);
                Assert.Equal("Usado", detalhe.DescricaoCondicao);
            },
            detalhe =>
            {
                Assert.Equal(jogo2.Id, detalhe.Id);
                Assert.Equal("Mega Drive", detalhe.NomePlataforma);
                Assert.Equal("Lacrado", detalhe.DescricaoCondicao);
            });
        Assert.Equal(pagina, resultado.Pagina);
        Assert.Equal(tamanhoPagina, resultado.TamanhoPagina);
        Assert.Equal(5, resultado.TotalItens);
        Assert.Equal(3, resultado.TotalPaginas);
        _jogos.Verify(r => r.ContarAsync(It.Is<JogoFiltro>(f =>
            f.Pagina == pagina && f.TamanhoPagina == tamanhoPagina)), Times.Once);
        _jogos.Verify(r => r.ListarAsync(It.Is<JogoFiltro>(f =>
            f.Pagina == pagina && f.TamanhoPagina == tamanhoPagina)), Times.Once);
    }

    [Fact]
    public async Task ObterAsync_IdInexistente_LancaJogoNaoEncontrado()
    {
        // Arrange
        const int idInexistente = 42;
        _jogos.Setup(r => r.ObterPorIdAsync(idInexistente)).ReturnsAsync((Jogo?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.ObterAsync(idInexistente));

        // Assert
        Assert.Equal(BusinessErrors.JogoNaoEncontrado, erro.Erro);
        _jogos.Verify(r => r.ObterPorIdAsync(idInexistente), Times.Once);
    }

    [Fact]
    public async Task DesativarAsync_IdInexistente_LancaJogoNaoEncontradoSemDesativar()
    {
        // Arrange
        const int idInexistente = 42;
        _jogos.Setup(r => r.ObterPorIdAsync(idInexistente)).ReturnsAsync((Jogo?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.DesativarAsync(idInexistente));

        // Assert
        Assert.Equal(BusinessErrors.JogoNaoEncontrado, erro.Erro);
        _jogos.Verify(r => r.ObterPorIdAsync(idInexistente), Times.Once);
        _jogos.Verify(r => r.DesativarAsync(It.IsAny<int>()), Times.Never);
    }

    private static Jogo NovoJogo() => new() { Nome = "Chrono Trigger", IdPlataforma = 1, IdCondicaoJogo = 2 };

    private void VerificarQueNaoHouveInsercao()
    {
        _jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>()), Times.Never);
        _jogosGeneros.Verify(r => r.InserirAsync(It.IsAny<JogoGenero>()), Times.Never);
    }
}
