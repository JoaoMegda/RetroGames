using MySqlConnector;
using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    private const string Colunas = "co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil";

    public UsuarioRepository(RepositoryConfig configuracao) : base(configuracao) { }

    private static Usuario Mapear(MySqlDataReader l) => new()
    {
        Id = l.GetInt32("co_seq_usuario"),
        Nome = l.GetString("no_usuario"),
        Email = l.GetString("ds_email"),
        SenhaHash = l.GetString("ds_senha_hash"),
        Ativo = l.GetBoolean("st_ativo"),
        DataCadastro = l.GetDateTime("dt_cadastro"),
        IdPerfil = l.GetInt32("co_perfil"),
    };

    public Task<Usuario?> ObterPorIdAsync(int id) =>
        ConsultarUmAsync($"SELECT {Colunas} FROM tb_usuario WHERE co_seq_usuario = @id",
            p => p.AddWithValue("@id", id), Mapear);

    public Task<Usuario?> ObterPorEmailAsync(string email) =>
        ConsultarUmAsync($"SELECT {Colunas} FROM tb_usuario WHERE ds_email = @email",
            p => p.AddWithValue("@email", email), Mapear);

    public async Task<int> InserirAsync(Usuario usuario)
    {
        var (idGerado, _) = await ExecutarGravacaoAsync(
            "INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil) " +
            "VALUES (@nome, @email, @senhaHash, @ativo, @dataCadastro, @idPerfil)",
            p =>
            {
                p.AddWithValue("@nome", usuario.Nome);
                p.AddWithValue("@email", usuario.Email);
                p.AddWithValue("@senhaHash", usuario.SenhaHash);
                p.AddWithValue("@ativo", usuario.Ativo);
                p.AddWithValue("@dataCadastro", usuario.DataCadastro);
                p.AddWithValue("@idPerfil", usuario.IdPerfil);
            });
        return (int)idGerado;
    }
}
