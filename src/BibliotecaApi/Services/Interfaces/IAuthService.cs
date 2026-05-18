using BibliotecaApi.DTOs;

namespace BibliotecaApi.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegistrarAsync(RegistrarUsuarioDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
