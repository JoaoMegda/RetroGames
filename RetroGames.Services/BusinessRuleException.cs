namespace RetroGames.Services;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(BusinessError erro) : base(erro.Resposta.DefaultMessage) => Erro = erro;

    public BusinessError Erro { get; }
}
