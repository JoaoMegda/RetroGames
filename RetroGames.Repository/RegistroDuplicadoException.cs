namespace RetroGames.Repository;

/// <summary>Lançada pelo Repository quando o MySQL recusa a gravação por UNIQUE (erro 1062).</summary>
public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(Exception? inner = null)
        : base("Registro duplicado.", inner) { }
}
