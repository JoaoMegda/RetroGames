using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class AutenticacaoServiceTests
{
    [Theory]
    [InlineData(false, false)] // E-mail inexistente
    [InlineData(true, false)]  // Usuário inativo
    [InlineData(true, true)]   // Senha incorreta
    public async Task AutenticarAsync_CredenciaisInvalidas_RetornaNulo(bool cadastrado, bool ativo)
    {
        const string email = "usuario@example.test";
        var usuarios = new Mock<IUsuarioRepository>();
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Usuario).Returns(usuarios.Object);
        var hasher = new PasswordHasher<Usuario>();
        var senha = Guid.NewGuid().ToString();
        var usuario = new Usuario { Email = email, Ativo = ativo };
        usuario.SenhaHash = hasher.HashPassword(usuario, senha);
        usuarios.Setup(r => r.ObterPorEmailAsync(email)).ReturnsAsync(cadastrado ? usuario : null);

        var service = new AutenticacaoService(repositorios.Object, hasher);
        var senhaInformada = ativo ? Guid.NewGuid().ToString() : senha;
        var resultado = await service.AutenticarAsync(email, senhaInformada);

        Assert.Null(resultado);
    }
}
