using RetroGames.Model;
using RetroGames.Model.Model;

namespace RetroGames.Services.ServiceModels;

/// <summary>Jogo composto com plataforma, condição e gêneros (montado com LINQ no JogoService).</summary>
public class JogoDetalheServiceModel
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
    public List<Genero> Generos { get; set; } = new();
}
