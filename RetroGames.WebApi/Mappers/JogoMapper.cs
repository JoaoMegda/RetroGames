using RetroGames.Model;
using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static Jogo ParaModel(InserirJogoRequestApiModel request) => new()
    {
        Nome = request.Nome.Trim(),
        Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim(),
        AnoLancamento = request.AnoLancamento,
        Preco = request.Preco,
        Estoque = request.Estoque,
        IdPlataforma = request.IdPlataforma,
        IdCondicaoJogo = request.IdCondicaoJogo,
    };

    public static Jogo ParaModel(int id, AlterarJogoRequestApiModel request) => new()
    {
        Id = id,
        Nome = request.Nome.Trim(),
        Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim(),
        AnoLancamento = request.AnoLancamento,
        Preco = request.Preco,
        Estoque = request.Estoque,
        IdPlataforma = request.IdPlataforma,
        IdCondicaoJogo = request.IdCondicaoJogo,
    };

    public static JogoFiltroServiceModel ParaServiceModel(ListarJogoRequestApiModel request) => new()
    {
        Busca = request.Busca,
        IdPlataforma = request.IdPlataforma,
        IdGenero = request.IdGenero,
        IncluirInativos = request.IncluirInativos,
        Pagina = request.Pagina,
        TamanhoPagina = request.TamanhoPagina,
    };

    public static ConsultarJogoResponseApiModel ParaResponse(JogoDetalheServiceModel jogo) => new()
    {
        Id = jogo.Id,
        Nome = jogo.Nome,
        Descricao = jogo.Descricao,
        AnoLancamento = jogo.AnoLancamento,
        Preco = jogo.Preco,
        Estoque = jogo.Estoque,
        Ativo = jogo.Ativo,
        DataCadastro = jogo.DataCadastro,
        IdPlataforma = jogo.IdPlataforma,
        NomePlataforma = jogo.NomePlataforma,
        IdCondicaoJogo = jogo.IdCondicaoJogo,
        DescricaoCondicao = jogo.DescricaoCondicao,
        Generos = jogo.Generos.Select(g => new ConsultarGeneroResponseApiModel { Id = g.Id, Nome = g.Nome }).ToList(),
    };

    public static ListarJogoResponseApiModel ParaResponse(ListaPaginadaServiceModel<JogoDetalheServiceModel> lista) => new()
    {
        Itens = lista.Itens.Select(ParaResponse).ToList(),
        Pagina = lista.Pagina,
        TamanhoPagina = lista.TamanhoPagina,
        TotalItens = lista.TotalItens,
        TotalPaginas = lista.TotalPaginas,
    };
}
