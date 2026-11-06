using Microsoft.AspNetCore.Identity;
using RetroGames.Model;
using RetroGames.Repository;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet _repositorios;
    private readonly IPasswordHasher<Usuario> _hasher;

    public AutenticacaoService(IRepositorySet repositorios, IPasswordHasher<Usuario> hasher)
    {
        _repositorios = repositorios;
        _hasher = hasher;
    }

    public async Task<UsuarioAutenticadoServiceModel> AutenticarAsync(string email, string senha)
    {
        var usuario = await _repositorios.Usuario.ObterPorEmailAsync(email);
        if (usuario is null || !usuario.Ativo ||
            _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha) == PasswordVerificationResult.Failed)
            throw new BusinessRuleException(BusinessErrors.CredenciaisInvalidas);

        return await MontarAsync(usuario);
    }

    public async Task<UsuarioAutenticadoServiceModel> ObterUsuarioAsync(int id)
    {
        var usuario = await _repositorios.Usuario.ObterPorIdAsync(id);
        if (usuario is null || !usuario.Ativo)
            throw new BusinessRuleException(BusinessErrors.NaoAutenticado);

        return await MontarAsync(usuario);
    }

    public async Task GarantirAdministradorInicialAsync(string email, string senha)
    {
        if (await _repositorios.Usuario.ObterPorEmailAsync(email) is not null) return;

        var usuario = new Usuario
        {
            Nome = "Administrador",
            Email = email,
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            IdPerfil = Perfis.IdAdministrador,
        };
        usuario.SenhaHash = _hasher.HashPassword(usuario, senha);

        try
        {
            await _repositorios.Usuario.InserirAsync(usuario);
        }
        catch (RegistroDuplicadoException)
        {
            // Outra instância criou o administrador entre a consulta e o INSERT: nada a fazer.
        }
    }

    private async Task<UsuarioAutenticadoServiceModel> MontarAsync(Usuario usuario)
    {
        var perfil = await _repositorios.Perfil.ObterPorIdAsync(usuario.IdPerfil);
        return new UsuarioAutenticadoServiceModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            IdPerfil = usuario.IdPerfil,
            DescricaoPerfil = perfil?.Descricao ?? string.Empty,
        };
    }
}
