using MySqlConnector;
using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas =
        "co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, " +
        "co_plataforma, co_condicao_jogo";

    public JogoRepository(RepositoryConfig configuracao) : base(configuracao) { }

    private static Jogo Mapear(MySqlDataReader l) => new()
    {
        Id = l.GetInt32("co_seq_jogo"),
        Nome = l.GetString("no_jogo"),
        Descricao = l.ObterStringOuNulo("ds_jogo"),
        AnoLancamento = l.GetInt32("nu_ano_lancamento"),
        Preco = l.GetDecimal("vl_preco"),
        Estoque = l.GetInt32("qt_estoque"),
        Ativo = l.GetBoolean("st_ativo"),
        DataCadastro = l.GetDateTime("dt_cadastro"),
        IdPlataforma = l.GetInt32("co_plataforma"),
        IdCondicaoJogo = l.GetInt32("co_condicao_jogo"),
    };

    /// <summary>Monta o WHERE (só texto fixo e nomes de parâmetros) e a ação que adiciona os valores.</summary>
    private static (string Where, Action<MySqlParameterCollection> Parametros) MontarFiltro(JogoFiltro filtro)
    {
        var condicoes = new List<string>();
        if (!filtro.IncluirInativos)
            condicoes.Add("st_ativo = 1");
        if (!string.IsNullOrWhiteSpace(filtro.Busca))
            condicoes.Add("(no_jogo LIKE @busca OR ds_jogo LIKE @busca)");
        if (filtro.IdPlataforma.HasValue)
            condicoes.Add("co_plataforma = @idPlataforma");
        if (filtro.IdsJogo is not null)
            condicoes.Add(filtro.IdsJogo.Count == 0
                ? "1 = 0"
                : $"co_seq_jogo IN ({NomesParametros("idJogo", filtro.IdsJogo.Count)})");

        var where = condicoes.Count == 0 ? string.Empty : " WHERE " + string.Join(" AND ", condicoes);

        void Parametros(MySqlParameterCollection p)
        {
            if (!string.IsNullOrWhiteSpace(filtro.Busca))
                p.AddWithValue("@busca", $"%{EscaparLike(filtro.Busca.Trim())}%");
            if (filtro.IdPlataforma.HasValue)
                p.AddWithValue("@idPlataforma", filtro.IdPlataforma.Value);
            if (filtro.IdsJogo is { Count: > 0 })
                AdicionarLista(p, "idJogo", filtro.IdsJogo);
        }

        return (where, Parametros);
    }

    private static string EscaparLike(string texto) =>
        texto.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

    public Task<List<Jogo>> ListarAsync(JogoFiltro filtro)
    {
        var (where, parametros) = MontarFiltro(filtro);
        return ConsultarAsync(
            $"SELECT {Colunas} FROM tb_jogo{where} ORDER BY no_jogo, co_seq_jogo LIMIT @limite OFFSET @deslocamento",
            p =>
            {
                parametros(p);
                p.AddWithValue("@limite", filtro.TamanhoPagina);
                p.AddWithValue("@deslocamento", (filtro.Pagina - 1) * filtro.TamanhoPagina);
            },
            Mapear);
    }

    public Task<int> ContarAsync(JogoFiltro filtro)
    {
        var (where, parametros) = MontarFiltro(filtro);
        return ContarAsync($"SELECT COUNT(*) FROM tb_jogo{where}", parametros);
    }

    public Task<Jogo?> ObterPorIdAsync(int id) =>
        ConsultarUmAsync($"SELECT {Colunas} FROM tb_jogo WHERE co_seq_jogo = @id",
            p => p.AddWithValue("@id", id), Mapear);

    public Task<int> ContarPorPlataformaAsync(int idPlataforma) =>
        ContarAsync("SELECT COUNT(*) FROM tb_jogo WHERE co_plataforma = @idPlataforma",
            p => p.AddWithValue("@idPlataforma", idPlataforma));

    public async Task<int> InserirAsync(Jogo jogo)
    {
        var (idGerado, _) = await ExecutarGravacaoAsync(
            "INSERT INTO tb_jogo (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, " +
            "co_plataforma, co_condicao_jogo) " +
            "VALUES (@nome, @descricao, @anoLancamento, @preco, @estoque, @ativo, @dataCadastro, @idPlataforma, @idCondicaoJogo)",
            p =>
            {
                p.AddWithValue("@nome", jogo.Nome);
                p.AddWithValue("@descricao", Valor(jogo.Descricao));
                p.AddWithValue("@anoLancamento", jogo.AnoLancamento);
                p.AddWithValue("@preco", jogo.Preco);
                p.AddWithValue("@estoque", jogo.Estoque);
                p.AddWithValue("@ativo", jogo.Ativo);
                p.AddWithValue("@dataCadastro", jogo.DataCadastro);
                p.AddWithValue("@idPlataforma", jogo.IdPlataforma);
                p.AddWithValue("@idCondicaoJogo", jogo.IdCondicaoJogo);
            });
        return (int)idGerado;
    }

    public async Task<bool> AlterarAsync(Jogo jogo) =>
        await ExecutarAsync(
            "UPDATE tb_jogo SET no_jogo = @nome, ds_jogo = @descricao, nu_ano_lancamento = @anoLancamento, " +
            "vl_preco = @preco, qt_estoque = @estoque, co_plataforma = @idPlataforma, co_condicao_jogo = @idCondicaoJogo " +
            "WHERE co_seq_jogo = @id",
            p =>
            {
                p.AddWithValue("@id", jogo.Id);
                p.AddWithValue("@nome", jogo.Nome);
                p.AddWithValue("@descricao", Valor(jogo.Descricao));
                p.AddWithValue("@anoLancamento", jogo.AnoLancamento);
                p.AddWithValue("@preco", jogo.Preco);
                p.AddWithValue("@estoque", jogo.Estoque);
                p.AddWithValue("@idPlataforma", jogo.IdPlataforma);
                p.AddWithValue("@idCondicaoJogo", jogo.IdCondicaoJogo);
            }) > 0;

    // RN03: exclusão lógica
    public async Task<bool> DesativarAsync(int id) =>
        await ExecutarAsync("UPDATE tb_jogo SET st_ativo = 0 WHERE co_seq_jogo = @id",
            p => p.AddWithValue("@id", id)) > 0;
}
