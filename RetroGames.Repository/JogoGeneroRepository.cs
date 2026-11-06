using MySqlConnector;
using RetroGames.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(ConfiguracaoBanco configuracao) : base(configuracao) { }

    private static JogoGenero Mapear(MySqlDataReader l) => new()
    {
        IdJogo = l.GetInt32("co_jogo"),
        IdGenero = l.GetInt32("co_genero"),
    };

    public async Task<List<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogo)
    {
        if (idsJogo.Count == 0) return new List<JogoGenero>();
        var nomes = NomesParametros("idJogo", idsJogo.Count);
        return await ConsultarAsync(
            $"SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN ({nomes})",
            p => AdicionarLista(p, "idJogo", idsJogo), Mapear);
    }

    public Task<List<JogoGenero>> ListarPorGeneroAsync(int idGenero) =>
        ConsultarAsync("SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_genero = @idGenero",
            p => p.AddWithValue("@idGenero", idGenero), Mapear);

    public Task InserirAsync(JogoGenero jogoGenero) =>
        ExecutarAsync("INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES (@idJogo, @idGenero)",
            p =>
            {
                p.AddWithValue("@idJogo", jogoGenero.IdJogo);
                p.AddWithValue("@idGenero", jogoGenero.IdGenero);
            });

    public Task ExcluirPorJogoAsync(int idJogo) =>
        ExecutarAsync("DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo",
            p => p.AddWithValue("@idJogo", idJogo));
}
