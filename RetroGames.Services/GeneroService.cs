using RetroGames.Model;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class GeneroService : IGeneroService
{
    private readonly IRepositorySet _repositorios;

    public GeneroService(IRepositorySet repositorios) => _repositorios = repositorios;

    public Task<List<Genero>> ListarAsync() => _repositorios.Genero.ListarAsync();
}
