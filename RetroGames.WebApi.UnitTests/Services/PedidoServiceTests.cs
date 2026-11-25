using System.Transactions;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using RetroGames.Services.ServiceModels;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class PedidoServiceTests
{
    private const int IdUsuario = 37;
    private readonly Mock<IPedidoRepository> _pedidos = new(MockBehavior.Strict);
    private readonly Mock<IItemPedidoRepository> _itens = new(MockBehavior.Strict);
    private readonly Mock<ISituacaoPedidoRepository> _situacoes = new(MockBehavior.Strict);
    private readonly Mock<IJogoRepository> _jogos = new(MockBehavior.Strict);
    private readonly Mock<IUsuarioAtualService> _usuarioAtual = new();
    private readonly PedidoService _servico;

    public PedidoServiceTests()
    {
        _usuarioAtual.SetupGet(u => u.EstaAutenticado).Returns(true);
        _usuarioAtual.SetupGet(u => u.Id).Returns(IdUsuario);
        _servico = new PedidoService(_pedidos.Object, _itens.Object, _situacoes.Object, _jogos.Object, _usuarioAtual.Object);
    }

    [Fact]
    public async Task InserirEListarMeus_SemSessao_LancamNaoAutenticado()
    {
        _usuarioAtual.SetupGet(u => u.EstaAutenticado).Returns(false);

        var erroInserir = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync([new(1, 1)]));
        var erroListar = await Assert.ThrowsAsync<BusinessRuleException>(_servico.ListarMeusAsync);

        Assert.Equal(BusinessErrors.NaoAutenticado, erroInserir.Erro);
        Assert.Equal(BusinessErrors.NaoAutenticado, erroListar.Erro);
    }

    [Fact]
    public async Task ListarMeus_SemIdDoUsuario_LancaNaoAutenticado()
    {
        _usuarioAtual.SetupGet(u => u.Id).Returns((int?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(_servico.ListarMeusAsync);

        Assert.Equal(BusinessErrors.NaoAutenticado, erro.Erro);
    }

    [Fact]
    public async Task Inserir_ComItensVaziosOuNulos_LancaPedidoItensInvalidos()
    {
        var erroVazio = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync([]));
        var erroNulo = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync(null!));
        var erroItemNulo = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync([null!]));

        Assert.Equal(3, erroVazio.Erro.Resposta.Code);
        Assert.Equal(3, erroNulo.Erro.Resposta.Code);
        Assert.Equal(3, erroItemNulo.Erro.Resposta.Code);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1, 11)]
    [InlineData(0, 1)]
    public async Task Inserir_ComIdOuQuantidadeInvalidos_LancaPedidoItensInvalidos(int idJogo, int quantidade)
    {
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync([new(idJogo, quantidade)]));

        Assert.Equal(3, erro.Erro.Resposta.Code);
    }

    [Fact]
    public async Task Inserir_ComJogoRepetido_LancaPedidoItensInvalidos()
    {
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync([new(4, 1), new(4, 2)]));

        Assert.Equal(3, erro.Erro.Resposta.Code);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Inserir_ComJogoInexistenteOuInativo_LancaJogoIndisponivel(bool jogoExiste)
    {
        if (jogoExiste)
            PrepararJogoParaCompra(4, "Chrono Trigger", 25m, estoque: 5, ativo: false);
        else
            _jogos.Setup(r => r.ObterParaCompraAsync(4)).ReturnsAsync((Jogo?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync([new(4, 1)]));

        Assert.Equal(110, erro.Erro.Resposta.Code);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, erro.Erro.Status);
    }

    [Fact]
    public async Task Inserir_ComEstoqueMenorQueQuantidade_LancaEstoqueInsuficiente()
    {
        PrepararJogoParaCompra(4, "Chrono Trigger", 25m, estoque: 1);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync([new(4, 2)]));

        Assert.Equal(111, erro.Erro.Resposta.Code);
        Assert.Equal(System.Net.HttpStatusCode.Conflict, erro.Erro.Status);
    }

    [Fact]
    public async Task Inserir_ComJogosDisponiveis_GravaPedidoPendenteComPrecosDoBanco()
    {
        var ordemBloqueios = new List<int>();
        PrepararJogoParaCompra(4, "Chrono Trigger", 12.50m, estoque: 8, aoLer: ordemBloqueios.Add);
        PrepararJogoParaCompra(7, "Super Metroid", 19.99m, estoque: 1, aoLer: ordemBloqueios.Add);
        PrepararPedidoPendente();
        var antes = DateTime.UtcNow.AddSeconds(-1);
        Pedido? pedidoGravado = null;
        _pedidos.Setup(r => r.InserirAsync(It.IsAny<Pedido>()))
            .Callback<Pedido>(pedido =>
            {
                Assert.Equal(IsolationLevel.ReadCommitted, Transaction.Current?.IsolationLevel);
                pedidoGravado = pedido;
            })
            .ReturnsAsync(82);
        _itens.Setup(r => r.InserirAsync(It.Is<ItemPedido>(item =>
            item.IdPedido == 82 && item.IdJogo == 4 && item.Quantidade == 2 && item.ValorUnitario == 12.50m)))
            .Returns(Task.CompletedTask);
        _itens.Setup(r => r.InserirAsync(It.Is<ItemPedido>(item =>
            item.IdPedido == 82 && item.IdJogo == 7 && item.Quantidade == 1 && item.ValorUnitario == 19.99m)))
            .Returns(Task.CompletedTask);
        _jogos.Setup(r => r.BaixarEstoqueAsync(4, 2)).ReturnsAsync(true);
        _jogos.Setup(r => r.BaixarEstoqueAsync(7, 1)).ReturnsAsync(true);

        var resultado = await _servico.InserirAsync([new(7, 1), new(4, 2)]);

        Assert.Equal(new[] { 4, 7 }, ordemBloqueios);
        Assert.NotNull(pedidoGravado);
        Assert.Equal(82, resultado.Id);
        Assert.Equal(IdUsuario, pedidoGravado.IdUsuario);
        Assert.Equal(1, pedidoGravado.IdSituacaoPedido);
        Assert.Equal("Pendente", resultado.Situacao);
        Assert.Equal(DateTimeKind.Utc, pedidoGravado.Data.Kind);
        Assert.InRange(pedidoGravado.Data, antes, DateTime.UtcNow);
        Assert.Equal(pedidoGravado.Data, resultado.Data);
        Assert.Equal(44.99m, pedidoGravado.Total);
        Assert.Equal(44.99m, resultado.Total);
        Assert.Contains(resultado.Itens, item =>
            item.IdJogo == 4 && item.NomeJogo == "Chrono Trigger" && item.Quantidade == 2 && item.ValorUnitario == 12.50m);
        Assert.Contains(resultado.Itens, item =>
            item.IdJogo == 7 && item.NomeJogo == "Super Metroid" && item.Quantidade == 1 && item.ValorUnitario == 19.99m);
        _itens.Verify(r => r.InserirAsync(It.IsAny<ItemPedido>()), Times.Exactly(2));
        _jogos.Verify(r => r.BaixarEstoqueAsync(4, 2), Times.Once);
        _jogos.Verify(r => r.BaixarEstoqueAsync(7, 1), Times.Once);
    }

    [Fact]
    public async Task Inserir_QuandoSegundaBaixaNaoAfetaLinhas_AbortaTransacao()
    {
        PrepararJogoParaCompra(4, "Chrono Trigger", 12.50m, estoque: 8);
        PrepararJogoParaCompra(7, "Super Metroid", 19.99m, estoque: 1);
        PrepararPedidoPendente();
        TransactionStatus? statusAoConcluir = null;
        _pedidos.Setup(r => r.InserirAsync(It.IsAny<Pedido>()))
            .Callback<Pedido>(_ =>
            {
                var transacao = Transaction.Current;
                Assert.NotNull(transacao);
                transacao.TransactionCompleted += (_, evento) =>
                    statusAoConcluir = evento.Transaction?.TransactionInformation.Status;
            })
            .ReturnsAsync(82);
        _itens.Setup(r => r.InserirAsync(It.IsAny<ItemPedido>())).Returns(Task.CompletedTask);
        _jogos.Setup(r => r.BaixarEstoqueAsync(4, 2)).ReturnsAsync(true);
        _jogos.Setup(r => r.BaixarEstoqueAsync(7, 1)).ReturnsAsync(false);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => _servico.InserirAsync([new(4, 2), new(7, 1)]));

        Assert.Equal(111, erro.Erro.Resposta.Code);
        Assert.Equal(TransactionStatus.Aborted, statusAoConcluir);
        _jogos.Verify(r => r.BaixarEstoqueAsync(4, 2), Times.Once);
    }

    [Fact]
    public async Task ListarMeus_SemPedidos_DevolveListaVazia()
    {
        _pedidos.Setup(r => r.ListarPorUsuarioAsync(IdUsuario)).ReturnsAsync([]);

        var resultado = await _servico.ListarMeusAsync();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ListarMeus_ConsultaUsuarioLogadoEMantemPrecoHistoricoDosItens()
    {
        var recente = new DateTime(2026, 11, 23, 16, 30, 0, DateTimeKind.Utc);
        var antigo = recente.AddDays(-1);
        _pedidos.Setup(r => r.ListarPorUsuarioAsync(IdUsuario)).ReturnsAsync([
            new Pedido { Id = 18, IdUsuario = IdUsuario, Data = recente, Total = 50m, IdSituacaoPedido = 2 },
            new Pedido { Id = 12, IdUsuario = IdUsuario, Data = antigo, Total = 12m, IdSituacaoPedido = 1 },
        ]);
        _itens.Setup(r => r.ListarPorPedidosAsync(It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { 18, 12 }))))
            .ReturnsAsync([
                new ItemPedido { Id = 5, IdPedido = 12, IdJogo = 4, Quantidade = 1, ValorUnitario = 12m },
                new ItemPedido { Id = 11, IdPedido = 18, IdJogo = 4, Quantidade = 2, ValorUnitario = 25m },
            ]);
        _jogos.Setup(r => r.ListarPorIdsAsync(It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { 4 }))))
            .ReturnsAsync([new Jogo { Id = 4, Nome = "Chrono Trigger", Preco = 60m, Ativo = false, Estoque = 0 }]);
        _situacoes.Setup(r => r.ListarAsync()).ReturnsAsync([
            new SituacaoPedido { Id = 1, Descricao = "Pendente" },
            new SituacaoPedido { Id = 2, Descricao = "Pago" },
        ]);

        var resultado = await _servico.ListarMeusAsync();

        Assert.Collection(resultado,
            pedido =>
            {
                Assert.Equal(18, pedido.Id);
                Assert.Equal(recente, pedido.Data);
                Assert.Equal("Pago", pedido.Situacao);
                Assert.Equal(50m, pedido.Total);
                var item = Assert.Single(pedido.Itens);
                Assert.Equal("Chrono Trigger", item.NomeJogo);
                Assert.Equal(2, item.Quantidade);
                Assert.Equal(25m, item.ValorUnitario);
            },
            pedido =>
            {
                Assert.Equal(12, pedido.Id);
                Assert.Equal("Pendente", pedido.Situacao);
                Assert.Equal(12m, Assert.Single(pedido.Itens).ValorUnitario);
            });
    }

    private void PrepararJogoParaCompra(int id, string nome, decimal preco, int estoque, bool ativo = true, Action<int>? aoLer = null) =>
        _jogos.Setup(r => r.ObterParaCompraAsync(id))
            .Callback<int>(idLido => aoLer?.Invoke(idLido))
            .ReturnsAsync(new Jogo { Id = id, Nome = nome, Preco = preco, Estoque = estoque, Ativo = ativo });

    private void PrepararPedidoPendente() =>
        _situacoes.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new SituacaoPedido { Id = 1, Descricao = "Pendente" });
}
