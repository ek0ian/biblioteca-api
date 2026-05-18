using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using BibliotecaApi.Repositories.Interfaces;
using BibliotecaApi.Services.Interfaces;
using BibliotecaApi.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BibliotecaApi.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUsuarioRepository usuarioRepository, IOptions<JwtSettings> jwtSettings)
    {
        _usuarioRepository = usuarioRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegistrarAsync(RegistrarUsuarioDto dto)
    {
        if (await _usuarioRepository.EmailExisteAsync(dto.Email))
            throw new InvalidOperationException("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Perfil = PerfilUsuario.Usuario
        };

        var criado = await _usuarioRepository.CriarAsync(usuario);
        return GerarToken(criado);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        return GerarToken(usuario);
    }

    private AuthResponseDto GerarToken(Usuario usuario)
    {
        var expiracao = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id!),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiracao,
            signingCredentials: credenciais
        );

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil.ToString(),
            ExpiraEm = expiracao
        };
    }
}
