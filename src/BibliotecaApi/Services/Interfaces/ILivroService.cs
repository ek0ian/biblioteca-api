using BibliotecaApi.DTOs;

namespace BibliotecaApi.Services.Interfaces;

public interface ILivroService
{
    Task<List<LivroResponseDto>> ObterTodosAsync();
    Task<LivroResponseDto> ObterPorIdAsync(string id);
    Task<LivroResponseDto> CriarAsync(CriarLivroDto dto);
    Task<LivroResponseDto> AtualizarAsync(string id, AtualizarLivroDto dto);
    Task DeletarAsync(string id);
}
