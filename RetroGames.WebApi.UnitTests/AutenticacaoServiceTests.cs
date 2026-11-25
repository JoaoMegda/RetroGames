using System.Net;
using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests;

public class AutenticacaoServiceTests
{
    [Fact]
    public async Task CadastrarAsync_NovoCliente_GravaCredenciaisComHashERetornaUsuario()
    {
        const string email = "cliente@example.test";
        var senha = Guid.NewGuid().ToString();
        var usuarios = new Mock<IUsuarioRepository>();
        var perfis = new Mock<IPerfilRepository>();
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Usuario).Returns(usuarios.Object);
        repositorios.SetupGet(r => r.Perfil).Returns(perfis.Object);
        perfis.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(new Perfil { Id = 2, Descricao = "Cliente" });
        var hasher = new PasswordHasher<Usuario>();
        Usuario? usuarioGravado = null;
        usuarios.Setup(r => r.InserirAsync(It.IsAny<Usuario>()))
            .Callback<Usuario>(usuario => usuarioGravado = usuario)
            .ReturnsAsync(41);
        var service = new AutenticacaoService(repositorios.Object, hasher);

        var resultado = await service.CadastrarAsync(" Cliente Teste ", $" {email} ", senha);

        Assert.NotNull(usuarioGravado);
        Assert.True(usuarioGravado.Ativo);
        Assert.Equal(DateTimeKind.Utc, usuarioGravado.DataCadastro.Kind);
        Assert.NotEqual(senha, usuarioGravado.SenhaHash);
        Assert.Equal(PasswordVerificationResult.Success,
            hasher.VerifyHashedPassword(usuarioGravado, usuarioGravado.SenhaHash, senha));
        Assert.Equal(41, resultado.Id);
        Assert.Equal("Cliente Teste", resultado.Nome);
        Assert.Equal(email, resultado.Email);
        Assert.Equal(2, resultado.IdPerfil);
        Assert.Equal("Cliente", resultado.DescricaoPerfil);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CadastrarAsync_EmailEmUso_ConverteConflitoEmErroDeNegocio(bool jaExistiaNaConsulta)
    {
        const string email = "cliente@example.test";
        var usuarios = new Mock<IUsuarioRepository>();
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Usuario).Returns(usuarios.Object);
        usuarios.Setup(r => r.ObterPorEmailAsync(email))
            .ReturnsAsync(jaExistiaNaConsulta ? new Usuario { Email = email } : null);
        usuarios.Setup(r => r.InserirAsync(It.IsAny<Usuario>()))
            .ThrowsAsync(new RegistroDuplicadoException());
        var service = new AutenticacaoService(repositorios.Object, new PasswordHasher<Usuario>());

        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CadastrarAsync("Cliente Teste", email, Guid.NewGuid().ToString()));

        Assert.Equal(HttpStatusCode.Conflict, excecao.Erro.Status);
        Assert.Equal(106, excecao.Erro.Resposta.Code);
        usuarios.Verify(r => r.InserirAsync(It.IsAny<Usuario>()),
            jaExistiaNaConsulta ? Times.Never() : Times.Once());
    }

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
