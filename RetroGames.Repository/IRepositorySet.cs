namespace RetroGames.Repository;

/// <summary>Agregador dos repositórios (um por tabela), injetado nos Services.</summary>
public interface IRepositorySet
{
    ICondicaoJogoRepository CondicaoJogo { get; }
    IGeneroRepository Genero { get; }
    IJogoGeneroRepository JogoGenero { get; }
    IJogoRepository Jogo { get; }
    IPerfilRepository Perfil { get; }
    IPlataformaRepository Plataforma { get; }
    IUsuarioRepository Usuario { get; }
}
