namespace RetroGames.WebApi.ApiModels;

public class InserirJogoRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }
    public List<int> IdsGenero { get; set; } = new();
}

public class AlterarJogoRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }
    public List<int> IdsGenero { get; set; } = new();
}

/// <summary>Parâmetros de query do catálogo (GET /api/Jogo).</summary>
public class ListarJogoRequestApiModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public bool IncluirInativos { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
}

/// <summary>Campos resumidos de um jogo na listagem do catálogo.</summary>
public class ListarJogoResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
    public string NomePlataforma { get; set; } = string.Empty;
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<string> Generos { get; set; } = new();
}

/// <summary>Detalhe de um jogo consultado por id.</summary>
public class ListarPorIdJogoResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public int IdPlataforma { get; set; }
    public string NomePlataforma { get; set; } = string.Empty;
    public int IdCondicaoJogo { get; set; }
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<ConsultarGeneroResponseApiModel> Generos { get; set; } = new();
}

public class PaginaResponseApiModel<T>
{
    public List<T> Itens { get; set; } = new();
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalItens { get; set; }
    public int TotalPaginas { get; set; }
}
