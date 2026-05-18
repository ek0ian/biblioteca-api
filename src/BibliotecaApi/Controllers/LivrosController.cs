using BibliotecaApi.DTOs;
using BibliotecaApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LivrosController : ControllerBase
{
    private readonly ILivroService _livroService;

    public LivrosController(ILivroService livroService)
    {
        _livroService = livroService;
    }

    /// <summary>Lista todos os livros</summary>
    /// <response code="200">Lista de livros retornada com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<LivroResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
    {
        var livros = await _livroService.ObterTodosAsync();
        return Ok(livros);
    }

    /// <summary>Busca um livro pelo ID</summary>
    /// <param name="id">ID do livro (ObjectId MongoDB)</param>
    /// <response code="200">Livro encontrado</response>
    /// <response code="404">Livro não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LivroResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var livro = await _livroService.ObterPorIdAsync(id);
        return Ok(livro);
    }

    /// <summary>Cadastra um novo livro (somente Admin)</summary>
    /// <response code="201">Livro criado com sucesso</response>
    /// <response code="400">Dados inválidos ou ISBN duplicado</response>
    /// <response code="401">Não autenticado</response>
    /// <response code="403">Sem permissão (requer Admin)</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LivroResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Criar([FromBody] CriarLivroDto dto)
    {
        var livro = await _livroService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = livro.Id }, livro);
    }

    /// <summary>Atualiza dados de um livro (somente Admin)</summary>
    /// <param name="id">ID do livro</param>
    /// <response code="200">Livro atualizado</response>
    /// <response code="404">Livro não encontrado</response>
    /// <response code="403">Sem permissão (requer Admin)</response>
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LivroResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Atualizar(string id, [FromBody] AtualizarLivroDto dto)
    {
        var livro = await _livroService.AtualizarAsync(id, dto);
        return Ok(livro);
    }

    /// <summary>Remove um livro (somente Admin)</summary>
    /// <param name="id">ID do livro</param>
    /// <response code="204">Livro removido</response>
    /// <response code="404">Livro não encontrado</response>
    /// <response code="403">Sem permissão (requer Admin)</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Deletar(string id)
    {
        await _livroService.DeletarAsync(id);
        return NoContent();
    }
}
