using MySqlConnector;

namespace RetroGames.Repository;

/// <summary>Helpers ADO.NET. Cada chamada abre a própria conexão; dentro de um TransactionScope ela participa da transação do Service.</summary>
public abstract class RepositoryBase
{
    private readonly string _stringConexao;

    protected RepositoryBase(RepositoryConfig configuracao) => _stringConexao = configuracao.StringConexao;

    protected static object Valor(object? valor) => valor ?? DBNull.Value;

    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_stringConexao);
        await conexao.OpenAsync();
        return conexao;
    }

    protected static MySqlCommand CriarComando(MySqlConnection conexao, string sql) => new(sql, conexao);

    protected static void AdicionarParametro(MySqlParameterCollection parametros, string nome, object? valor) =>
        parametros.AddWithValue(nome, Valor(valor));

    protected async Task<List<T>> ConsultarAsync<T>(
        string sql, Action<MySqlParameterCollection>? parametros, Func<MySqlDataReader, T> mapear)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        parametros?.Invoke(comando.Parameters);
        await using var leitor = (MySqlDataReader)await comando.ExecuteReaderAsync();
        var itens = new List<T>();
        while (await leitor.ReadAsync())
            itens.Add(mapear(leitor));
        return itens;
    }

    protected async Task<T?> ConsultarUmAsync<T>(
        string sql, Action<MySqlParameterCollection>? parametros, Func<MySqlDataReader, T> mapear) where T : class
    {
        var itens = await ConsultarAsync(sql, parametros, mapear);
        return itens.FirstOrDefault();
    }

    protected async Task<int> ContarAsync(string sql, Action<MySqlParameterCollection>? parametros)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        parametros?.Invoke(comando.Parameters);
        return Convert.ToInt32(await comando.ExecuteScalarAsync());
    }

    /// <summary>INSERT/UPDATE/DELETE sem UNIQUE envolvido. Retorna as linhas afetadas.</summary>
    protected async Task<int> ExecutarAsync(string sql, Action<MySqlParameterCollection>? parametros)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        parametros?.Invoke(comando.Parameters);
        return await comando.ExecuteNonQueryAsync();
    }

    /// <summary>INSERT/UPDATE em tabela com UNIQUE: converte o erro 1062 em RegistroDuplicadoException.</summary>
    protected async Task<(long IdGerado, int LinhasAfetadas)> ExecutarGravacaoAsync(
        string sql, Action<MySqlParameterCollection> parametros)
    {
        try
        {
            await using var conexao = await AbrirConexaoAsync();
            await using var comando = CriarComando(conexao, sql);
            parametros(comando.Parameters);
            var linhas = await comando.ExecuteNonQueryAsync();
            return (comando.LastInsertedId, linhas);
        }
        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            throw new RegistroDuplicadoException(ex);
        }
    }

    /// <summary>Gera "@prefixo0, @prefixo1, ..." (apenas nomes de parâmetros, nunca valores).</summary>
    protected static string NomesParametros(string prefixo, int quantidade) =>
        string.Join(", ", Enumerable.Range(0, quantidade).Select(i => $"@{prefixo}{i}"));

    /// <summary>Adiciona os valores como parâmetros @prefixo0, @prefixo1, ...</summary>
    protected static void AdicionarLista(MySqlParameterCollection parametros, string prefixo, IReadOnlyCollection<int> valores)
    {
        var i = 0;
        foreach (var valor in valores)
            AdicionarParametro(parametros, $"@{prefixo}{i++}", valor);
    }
}

public static class LeitorExtensoes
{
    public static string? ObterStringOuNulo(this MySqlDataReader leitor, string coluna)
        => leitor.IsDBNull(leitor.GetOrdinal(coluna)) ? null : leitor.GetString(coluna);
}
