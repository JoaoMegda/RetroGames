using MySqlConnector;
using RetroGames.Model;

namespace RetroGames.Repository;

public class PerfilRepository : RepositoryBase, IPerfilRepository
{
    public PerfilRepository(ConfiguracaoBanco configuracao) : base(configuracao) { }

    public Task<Perfil?> ObterPorIdAsync(int id) =>
        ConsultarUmAsync("SELECT co_perfil, ds_perfil FROM tb_perfil WHERE co_perfil = @id",
            p => p.AddWithValue("@id", id),
            l => new Perfil { Id = l.GetInt32("co_perfil"), Descricao = l.GetString("ds_perfil") });
}
