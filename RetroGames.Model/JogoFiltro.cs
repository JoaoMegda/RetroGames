namespace RetroGames.Model;

/// <summary>Parâmetros da consulta paginada de tb_jogo (aplicados no SQL).</summary>
public class JogoFiltro
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }

    /// <summary>Quando não nulo, restringe a consulta a estes ids (resolvidos pelo Service). Lista vazia = nenhum resultado.</summary>
    public IReadOnlyCollection<int>? IdsJogo { get; set; }

    public bool IncluirInativos { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
}
