using MySqlConnector;
using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class CondicaoJogoRepository : RepositoryBase, ICondicaoJogoRepository
{
    private const string Colunas = "co_condicao_jogo, ds_condicao_jogo";

    public CondicaoJogoRepository(ConfiguracaoBanco configuracao) : base(configuracao) { }

    private static CondicaoJogo Mapear(MySqlDataReader l) => new()
    {
        Id = l.GetInt32("co_condicao_jogo"),
        Descricao = l.GetString("ds_condicao_jogo"),
    };

    public Task<List<CondicaoJogo>> ListarAsync() =>
        ConsultarAsync($"SELECT {Colunas} FROM tb_condicao_jogo ORDER BY co_condicao_jogo", null, Mapear);

    public Task<CondicaoJogo?> ObterPorIdAsync(int id) =>
        ConsultarUmAsync($"SELECT {Colunas} FROM tb_condicao_jogo WHERE co_condicao_jogo = @id",
            p => p.AddWithValue("@id", id), Mapear);
}
