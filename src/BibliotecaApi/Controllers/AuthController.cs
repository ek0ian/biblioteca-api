using BibliotecaApi.DTOs;
using BibliotecaApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Registra um novo usuário</summary>
    /// <response code="201">Usuário criado com token JWT</response>
    /// <response code="400">Dados inválidos ou e-mail já cadastrado</response>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
    {
        var resultado = await _authService.RegistrarAsync(dto);
        return CreatedAtAction(nameof(Registrar), resultado);
    }

    /// <summary>Realiza login e retorna token JWT</summary>
    /// <response code="200">Login bem-sucedido com token JWT</response>
    /// <response code="401">Credenciais inválidas</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var resultado = await _authService.LoginAsync(dto);
        return Ok(resultado);
    }
}
