using MySqlConnector;
using RetroGames.Model;

namespace RetroGames.Repository;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(ConfiguracaoBanco configuracao) : base(configuracao) { }

    private static Genero Mapear(MySqlDataReader l) => new()
    {
        Id = l.GetInt32("co_seq_genero"),
        Nome = l.GetString("no_genero"),
    };

    public Task<List<Genero>> ListarAsync() =>
        ConsultarAsync("SELECT co_seq_genero, no_genero FROM tb_genero ORDER BY no_genero", null, Mapear);

    public async Task<List<Genero>> ListarPorIdsAsync(IReadOnlyCollection<int> ids)
    {
        if (ids.Count == 0) return new List<Genero>();
        var nomes = NomesParametros("idGenero", ids.Count);
        return await ConsultarAsync(
            $"SELECT co_seq_genero, no_genero FROM tb_genero WHERE co_seq_genero IN ({nomes}) ORDER BY no_genero",
            p => AdicionarLista(p, "idGenero", ids), Mapear);
    }
}
