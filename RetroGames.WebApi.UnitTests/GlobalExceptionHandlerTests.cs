using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RetroGames.Services;
using RetroGames.WebApi.Handlers;
using System.Text.Json;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_RegraDeNegocio_DevolveStatusECatalogoDoErro()
    {
        var logger = new Mock<ILogger<GlobalExceptionHandler>>();
        var handler = new GlobalExceptionHandler(logger.Object);
        var contexto = NovoContexto();

        var tratado = await handler.TryHandleAsync(
            contexto,
            new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada),
            TestContext.Current.CancellationToken);

        contexto.Response.Body.Position = 0;
        using var json = await JsonDocument.ParseAsync(contexto.Response.Body, cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(tratado);
        Assert.Equal(StatusCodes.Status404NotFound, contexto.Response.StatusCode);
        Assert.Equal(100, json.RootElement.GetProperty("responseCode").GetInt32());
        Assert.Equal("Plataforma não encontrada.", json.RootElement.GetProperty("errorMessage").GetString());
    }

    [Fact]
    public async Task TryHandleAsync_ExcecaoNaoTratada_DevolveErroGenericoERegistraLogCompleto()
    {
        var logger = new Mock<ILogger<GlobalExceptionHandler>>();
        var handler = new GlobalExceptionHandler(logger.Object);
        var contexto = NovoContexto();
        var excecao = new InvalidOperationException("detalhe interno confidencial");

        var tratado = await handler.TryHandleAsync(contexto, excecao, TestContext.Current.CancellationToken);

        contexto.Response.Body.Position = 0;
        using var json = await JsonDocument.ParseAsync(contexto.Response.Body, cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(tratado);
        Assert.Equal(StatusCodes.Status500InternalServerError, contexto.Response.StatusCode);
        Assert.Equal(1, json.RootElement.GetProperty("responseCode").GetInt32());
        Assert.Equal(GlobalExceptionHandler.MensagemErroPadrao, json.RootElement.GetProperty("errorMessage").GetString());
        Assert.DoesNotContain("detalhe interno confidencial", json.RootElement.ToString());
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((estado, _) => estado.ToString()!.Contains("Erro não tratado.")),
                excecao,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static DefaultHttpContext NovoContexto()
    {
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();
        return contexto;
    }
}
