using BibliotecaApi.DTOs;

namespace BibliotecaApi.Services.Interfaces;

public interface IEmprestimoService
{
    Task<List<EmprestimoResponseDto>> ObterTodosAsync();
    Task<List<EmprestimoResponseDto>> ObterMeusEmprestimosAsync(string usuarioId);
    Task<EmprestimoResponseDto> ObterPorIdAsync(string id);
    Task<EmprestimoResponseDto> CriarAsync(CriarEmprestimoDto dto, string usuarioId);
    Task<EmprestimoResponseDto> DevolverAsync(string id, DevolverLivroDto dto);
    Task DeletarAsync(string id);
}
