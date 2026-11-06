using RetroGames.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PlataformaMapper
{
    public static Plataforma ParaModel(InserirPlataformaRequestApiModel request) => new()
    {
        Nome = request.Nome.Trim(),
        Fabricante = request.Fabricante.Trim(),
        AnoLancamento = request.AnoLancamento,
    };

    public static Plataforma ParaModel(int id, AlterarPlataformaRequestApiModel request) => new()
    {
        Id = id,
        Nome = request.Nome.Trim(),
        Fabricante = request.Fabricante.Trim(),
        AnoLancamento = request.AnoLancamento,
    };

    public static ConsultarPlataformaResponseApiModel ParaResponse(Plataforma plataforma) => new()
    {
        Id = plataforma.Id,
        Nome = plataforma.Nome,
        Fabricante = plataforma.Fabricante,
        AnoLancamento = plataforma.AnoLancamento,
    };
}
