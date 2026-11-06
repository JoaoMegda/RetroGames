using RetroGames.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class CondicaoJogoService : ICondicaoJogoService
{
    private readonly IRepositorySet _repositorios;

    public CondicaoJogoService(IRepositorySet repositorios) => _repositorios = repositorios;

    public Task<List<CondicaoJogo>> ListarAsync() => _repositorios.CondicaoJogo.ListarAsync();
}
