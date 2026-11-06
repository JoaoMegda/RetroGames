using System.Transactions;
using RetroGames.Model;
using RetroGames.Repository;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorios;

    public JogoService(IRepositorySet repositorios) => _repositorios = repositorios;

    // RN04: inativos só para o Administrador
    public async Task<ListaPaginadaServiceModel<JogoDetalheServiceModel>> ListarAsync(
        JogoFiltroServiceModel filtro, bool ehAdministrador)
    {
        var filtroRepositorio = new JogoFiltro
        {
            Busca = filtro.Busca,
            IdPlataforma = filtro.IdPlataforma,
            IncluirInativos = filtro.IncluirInativos && ehAdministrador,
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
        };

        // Filtro por gênero depende de outra tabela: resolvido aqui com LINQ e enviado ao SQL como lista de ids.
        if (filtro.IdGenero.HasValue)
        {
            var associacoes = await _repositorios.JogoGenero.ListarPorGeneroAsync(filtro.IdGenero.Value);
            filtroRepositorio.IdsJogo = associacoes.Select(a => a.IdJogo).Distinct().ToList();
        }

        var total = await _repositorios.Jogo.ContarAsync(filtroRepositorio);
        var jogos = await _repositorios.Jogo.ListarAsync(filtroRepositorio);

        return new ListaPaginadaServiceModel<JogoDetalheServiceModel>
        {
            Itens = await MontarDetalhesAsync(jogos),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling(total / (double)filtro.TamanhoPagina),
        };
    }

    public async Task<JogoDetalheServiceModel> ObterAsync(int id, bool ehAdministrador)
    {
        var jogo = await _repositorios.Jogo.ObterPorIdAsync(id);
        if (jogo is null || (!jogo.Ativo && !ehAdministrador))
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);

        return (await MontarDetalhesAsync(new[] { jogo })).Single();
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGenero)
    {
        var idsDistintos = idsGenero.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, idsDistintos); // RN05

        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await _repositorios.Jogo.InserirAsync(jogo);
            foreach (var idGenero in idsDistintos)
                await _repositorios.JogoGenero.InserirAsync(new JogoGenero { IdJogo = jogo.Id, IdGenero = idGenero });
            transactionScope.Complete();
        }

        return await ObterAsync(jogo.Id, ehAdministrador: true);
    }

    public async Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGenero)
    {
        _ = await _repositorios.Jogo.ObterPorIdAsync(jogo.Id)
            ?? throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);

        var idsDistintos = idsGenero.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, idsDistintos); // RN05

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            await _repositorios.Jogo.AlterarAsync(jogo);
            await _repositorios.JogoGenero.ExcluirPorJogoAsync(jogo.Id);
            foreach (var idGenero in idsDistintos)
                await _repositorios.JogoGenero.InserirAsync(new JogoGenero { IdJogo = jogo.Id, IdGenero = idGenero });
            transactionScope.Complete();
        }

        return await ObterAsync(jogo.Id, ehAdministrador: true);
    }

    // RN03: exclusão lógica (st_ativo = 0), preservando o registro e as associações
    public async Task DesativarAsync(int id)
    {
        _ = await _repositorios.Jogo.ObterPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        await _repositorios.Jogo.DesativarAsync(id);
    }

    private async Task ValidarReferenciasAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneroDistintos)
    {
        if (await _repositorios.Plataforma.ObterPorIdAsync(jogo.IdPlataforma) is null)
            throw new BusinessRuleException(BusinessErrors.PlataformaInexistente);

        if (await _repositorios.CondicaoJogo.ObterPorIdAsync(jogo.IdCondicaoJogo) is null)
            throw new BusinessRuleException(BusinessErrors.CondicaoInexistente);

        var generos = await _repositorios.Genero.ListarPorIdsAsync(idsGeneroDistintos);
        if (idsGeneroDistintos.Count == 0 || generos.Count != idsGeneroDistintos.Count)
            throw new BusinessRuleException(BusinessErrors.GeneroInexistente);
    }

    /// <summary>Compõe jogos com plataforma, condição e gêneros usando LINQ (consultas de uma tabela cada).</summary>
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(IReadOnlyCollection<Jogo> jogos)
    {
        if (jogos.Count == 0) return new List<JogoDetalheServiceModel>();

        var associacoes = await _repositorios.JogoGenero.ListarPorJogosAsync(jogos.Select(j => j.Id).ToList());
        var plataformas = await _repositorios.Plataforma.ListarAsync();
        var condicoes = await _repositorios.CondicaoJogo.ListarAsync();
        var generos = await _repositorios.Genero.ListarAsync();

        return jogos.Select(j => new JogoDetalheServiceModel
        {
            Id = j.Id,
            Nome = j.Nome,
            Descricao = j.Descricao,
            AnoLancamento = j.AnoLancamento,
            Preco = j.Preco,
            Estoque = j.Estoque,
            Ativo = j.Ativo,
            DataCadastro = j.DataCadastro,
            IdPlataforma = j.IdPlataforma,
            NomePlataforma = plataformas.FirstOrDefault(p => p.Id == j.IdPlataforma)?.Nome ?? string.Empty,
            IdCondicaoJogo = j.IdCondicaoJogo,
            DescricaoCondicao = condicoes.FirstOrDefault(c => c.Id == j.IdCondicaoJogo)?.Descricao ?? string.Empty,
            Generos = associacoes.Where(a => a.IdJogo == j.Id)
                .Join(generos, a => a.IdGenero, g => g.Id, (_, g) => g)
                .OrderBy(g => g.Nome)
                .ToList(),
        }).ToList();
    }
}
