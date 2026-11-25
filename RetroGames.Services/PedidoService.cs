using System.Transactions;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidos;
    private readonly IItemPedidoRepository _itens;
    private readonly ISituacaoPedidoRepository _situacoes;
    private readonly IJogoRepository _jogos;
    private readonly IUsuarioAtualService _usuarioAtual;

    public PedidoService(IPedidoRepository pedidos, IItemPedidoRepository itens,
        ISituacaoPedidoRepository situacoes, IJogoRepository jogos, IUsuarioAtualService usuarioAtual)
    {
        _pedidos = pedidos;
        _itens = itens;
        _situacoes = situacoes;
        _jogos = jogos;
        _usuarioAtual = usuarioAtual;
    }

    public async Task<PedidoDetalheServiceModel> InserirAsync(IReadOnlyCollection<InserirItemPedidoServiceModel?>? itens)
    {
        var idUsuario = ObterIdUsuario();
        if (itens is null || itens.Count == 0 ||
            itens.Any(item => item is null || item.IdJogo <= 0 || item.Quantidade is < 1 or > 10) ||
            itens.Select(item => item!.IdJogo).Distinct().Count() != itens.Count)
        {
            throw new BusinessRuleException(BusinessErrors.PedidoItensInvalidos);
        }

        // Todas as compras bloqueiam os jogos na mesma ordem para reduzir o risco de deadlock.
        var itensOrdenados = itens.Select(item => item!).OrderBy(item => item.IdJogo).ToList();
        var jogos = new Dictionary<int, Jogo>();
        PedidoDetalheServiceModel detalhe;
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            foreach (var item in itensOrdenados)
            {
                var jogo = await _jogos.ObterParaCompraAsync(item.IdJogo);
                if (jogo is null || !jogo.Ativo)
                    throw new BusinessRuleException(BusinessErrors.JOGO_INDISPONIVEL);

                if (jogo.Estoque < item.Quantidade)
                    throw new BusinessRuleException(BusinessErrors.ESTOQUE_INSUFICIENTE);

                jogos.Add(jogo.Id, jogo);
            }

            var situacao = await _situacoes.ObterPorIdAsync(SituacaoPedidoIds.Pendente)
                ?? throw new InvalidOperationException("A situação Pendente não foi cadastrada.");

            var itensPedido = itensOrdenados.Select(item => new ItemPedido
            {
                IdJogo = item.IdJogo,
                Quantidade = item.Quantidade,
                ValorUnitario = jogos[item.IdJogo].Preco,
            }).ToList();

            var agora = DateTime.UtcNow;
            var pedido = new Pedido
            {
                IdUsuario = idUsuario,
                IdSituacaoPedido = situacao.Id,
                // DATETIME guarda a data com precisão de segundos.
                Data = agora.AddTicks(-(agora.Ticks % TimeSpan.TicksPerSecond)),
                Total = itensPedido.Sum(item => item.Quantidade * item.ValorUnitario),
            };
            pedido.Id = await _pedidos.InserirAsync(pedido);

            foreach (var item in itensPedido)
            {
                item.IdPedido = pedido.Id;
                try
                {
                    await _itens.InserirAsync(item);
                }
                catch (RegistroDuplicadoException)
                {
                    throw new BusinessRuleException(BusinessErrors.PedidoItensInvalidos);
                }

                if (!await _jogos.BaixarEstoqueAsync(item.IdJogo, item.Quantidade))
                    throw new BusinessRuleException(BusinessErrors.ESTOQUE_INSUFICIENTE);
            }

            detalhe = MontarDetalhe(pedido, situacao.Descricao, itensPedido, jogos);
            transactionScope.Complete();
        }

        return detalhe;
    }

    public async Task<List<PedidoDetalheServiceModel>> ListarMeusAsync()
    {
        var pedidos = await _pedidos.ListarPorUsuarioAsync(ObterIdUsuario());
        if (pedidos.Count == 0)
            return [];

        var itens = await _itens.ListarPorPedidosAsync(pedidos.Select(pedido => pedido.Id).ToList());
        var itensPorPedido = itens.ToLookup(item => item.IdPedido);
        var idsJogos = itens.Select(item => item.IdJogo).Distinct().ToList();
        var jogos = (await _jogos.ListarPorIdsAsync(idsJogos)).ToDictionary(jogo => jogo.Id);
        var situacoes = (await _situacoes.ListarAsync()).ToDictionary(situacao => situacao.Id);

        return pedidos.Select(pedido => MontarDetalhe(
            pedido, situacoes[pedido.IdSituacaoPedido].Descricao, itensPorPedido[pedido.Id], jogos)).ToList();
    }

    private int ObterIdUsuario()
    {
        if (!_usuarioAtual.EstaAutenticado || _usuarioAtual.Id is not int id)
            throw new BusinessRuleException(BusinessErrors.NaoAutenticado);

        return id;
    }

    private static PedidoDetalheServiceModel MontarDetalhe(Pedido pedido, string situacao,
        IEnumerable<ItemPedido> itens, IReadOnlyDictionary<int, Jogo> jogos) => new()
    {
        Id = pedido.Id,
        Data = pedido.Data,
        Situacao = situacao,
        Total = pedido.Total,
        Itens = itens.Select(item => new ItemPedidoDetalheServiceModel
        {
            IdJogo = item.IdJogo,
            NomeJogo = jogos[item.IdJogo].Nome,
            Quantidade = item.Quantidade,
            ValorUnitario = item.ValorUnitario,
        }).ToList(),
    };
}
