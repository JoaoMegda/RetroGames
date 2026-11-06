using RetroGames.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet _repositorios;

    public PlataformaService(IRepositorySet repositorios) => _repositorios = repositorios;

    public Task<List<Plataforma>> ListarAsync() => _repositorios.Plataforma.ListarAsync();

    public async Task<Plataforma> ObterAsync(int id) =>
        await _repositorios.Plataforma.ObterPorIdAsync(id)
        ?? throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);

    // RN01
    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        await GarantirNomeUnicoAsync(plataforma);
        try
        {
            plataforma.Id = await _repositorios.Plataforma.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }
        return plataforma;
    }

    public async Task<Plataforma> AlterarAsync(Plataforma plataforma)
    {
        await ObterAsync(plataforma.Id);
        await GarantirNomeUnicoAsync(plataforma);
        try
        {
            await _repositorios.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }
        return plataforma;
    }

    // RN02
    public async Task ExcluirAsync(int id)
    {
        await ObterAsync(id);
        if (await _repositorios.Jogo.ContarPorPlataformaAsync(id) > 0)
            throw new BusinessRuleException(BusinessErrors.PlataformaPossuiJogos);
        await _repositorios.Plataforma.ExcluirAsync(id);
    }

    private async Task GarantirNomeUnicoAsync(Plataforma plataforma)
    {
        var existente = await _repositorios.Plataforma.ObterPorNomeAsync(plataforma.Nome);
        if (existente is not null && existente.Id != plataforma.Id)
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
    }
}
