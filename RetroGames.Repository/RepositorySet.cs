namespace RetroGames.Repository;

public class RepositorySet : IRepositorySet
{
    public RepositorySet(
        ICondicaoJogoRepository condicaoJogo, IGeneroRepository genero, IJogoGeneroRepository jogoGenero,
        IJogoRepository jogo, IPerfilRepository perfil, IPlataformaRepository plataforma, IUsuarioRepository usuario)
    {
        CondicaoJogo = condicaoJogo;
        Genero = genero;
        JogoGenero = jogoGenero;
        Jogo = jogo;
        Perfil = perfil;
        Plataforma = plataforma;
        Usuario = usuario;
    }

    public ICondicaoJogoRepository CondicaoJogo { get; }
    public IGeneroRepository Genero { get; }
    public IJogoGeneroRepository JogoGenero { get; }
    public IJogoRepository Jogo { get; }
    public IPerfilRepository Perfil { get; }
    public IPlataformaRepository Plataforma { get; }
    public IUsuarioRepository Usuario { get; }
}
