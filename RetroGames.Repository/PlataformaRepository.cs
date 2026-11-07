using MySqlConnector;
using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    private const string Colunas = "co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento";

    public PlataformaRepository(RepositoryConfig configuracao) : base(configuracao) { }

    private static Plataforma Mapear(MySqlDataReader l) => new()
    {
        Id = l.GetInt32("co_seq_plataforma"),
        Nome = l.GetString("no_plataforma"),
        Fabricante = l.GetString("no_fabricante"),
        AnoLancamento = l.GetInt32("nu_ano_lancamento"),
    };

    public Task<List<Plataforma>> ListarAsync() =>
        ConsultarAsync($"SELECT {Colunas} FROM tb_plataforma ORDER BY no_plataforma", null, Mapear);

    public Task<Plataforma?> ObterPorIdAsync(int id) =>
        ConsultarUmAsync($"SELECT {Colunas} FROM tb_plataforma WHERE co_seq_plataforma = @id",
            p => p.AddWithValue("@id", id), Mapear);

    public Task<Plataforma?> ObterPorNomeAsync(string nome) =>
        ConsultarUmAsync($"SELECT {Colunas} FROM tb_plataforma WHERE no_plataforma = @nome",
            p => p.AddWithValue("@nome", nome), Mapear);

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        var (idGerado, _) = await ExecutarGravacaoAsync(
            "INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) " +
            "VALUES (@nome, @fabricante, @anoLancamento)",
            p =>
            {
                p.AddWithValue("@nome", plataforma.Nome);
                p.AddWithValue("@fabricante", plataforma.Fabricante);
                p.AddWithValue("@anoLancamento", plataforma.AnoLancamento);
            });
        return (int)idGerado;
    }

    public async Task<bool> AlterarAsync(Plataforma plataforma)
    {
        var (_, linhas) = await ExecutarGravacaoAsync(
            "UPDATE tb_plataforma SET no_plataforma = @nome, no_fabricante = @fabricante, " +
            "nu_ano_lancamento = @anoLancamento WHERE co_seq_plataforma = @id",
            p =>
            {
                p.AddWithValue("@id", plataforma.Id);
                p.AddWithValue("@nome", plataforma.Nome);
                p.AddWithValue("@fabricante", plataforma.Fabricante);
                p.AddWithValue("@anoLancamento", plataforma.AnoLancamento);
            });
        return linhas > 0;
    }

    public async Task<bool> ExcluirAsync(int id) =>
        await ExecutarAsync("DELETE FROM tb_plataforma WHERE co_seq_plataforma = @id",
            p => p.AddWithValue("@id", id)) > 0;
}
