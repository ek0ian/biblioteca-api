using System.Security.Claims;
using BibliotecaApi.DTOs;
using BibliotecaApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EmprestimosController : ControllerBase
{
    private readonly IEmprestimoService _emprestimoService;

    public EmprestimosController(IEmprestimoService emprestimoService)
    {
        _emprestimoService = emprestimoService;
    }

    /// <summary>Lista todos os empréstimos (somente Admin)</summary>
    /// <response code="200">Lista de empréstimos</response>
    /// <response code="403">Sem permissão</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<EmprestimoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
    {
        var emprestimos = await _emprestimoService.ObterTodosAsync();
        return Ok(emprestimos);
    }

    /// <summary>Lista empréstimos do usuário autenticado</summary>
    /// <response code="200">Meus empréstimos</response>
    [HttpGet("meus")]
    [ProducesResponseType(typeof(List<EmprestimoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterMeus()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)!;
        var emprestimos = await _emprestimoService.ObterMeusEmprestimosAsync(usuarioId);
        return Ok(emprestimos);
    }

    /// <summary>Busca um empréstimo pelo ID</summary>
    /// <param name="id">ID do empréstimo</param>
    /// <response code="200">Empréstimo encontrado</response>
    /// <response code="404">Empréstimo não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EmprestimoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var emprestimo = await _emprestimoService.ObterPorIdAsync(id);
        return Ok(emprestimo);
    }

    /// <summary>Realiza um novo empréstimo</summary>
    /// <response code="201">Empréstimo criado</response>
    /// <response code="400">Livro indisponível ou dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(EmprestimoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarEmprestimoDto dto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)!;
        var emprestimo = await _emprestimoService.CriarAsync(dto, usuarioId);
        return CreatedAtAction(nameof(ObterPorId), new { id = emprestimo.Id }, emprestimo);
    }

    /// <summary>Registra devolução de um livro</summary>
    /// <param name="id">ID do empréstimo</param>
    /// <response code="200">Devolução registrada</response>
    /// <response code="400">Empréstimo já devolvido</response>
    /// <response code="404">Empréstimo não encontrado</response>
    [HttpPatch("{id}/devolver")]
    [ProducesResponseType(typeof(EmprestimoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Devolver(string id, [FromBody] DevolverLivroDto dto)
    {
        var emprestimo = await _emprestimoService.DevolverAsync(id, dto);
        return Ok(emprestimo);
    }

    /// <summary>Remove um empréstimo (somente Admin)</summary>
    /// <param name="id">ID do empréstimo</param>
    /// <response code="204">Empréstimo removido</response>
    /// <response code="403">Sem permissão</response>
    /// <response code="404">Empréstimo não encontrado</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(string id)
    {
        await _emprestimoService.DeletarAsync(id);
        return NoContent();
    }
}
