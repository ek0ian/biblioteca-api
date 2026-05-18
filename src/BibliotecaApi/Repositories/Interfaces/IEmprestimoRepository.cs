using BibliotecaApi.Models;

namespace BibliotecaApi.Repositories.Interfaces;

public interface IEmprestimoRepository
{
    Task<List<Emprestimo>> ObterTodosAsync();
    Task<List<Emprestimo>> ObterPorUsuarioAsync(string usuarioId);
    Task<Emprestimo?> ObterPorIdAsync(string id);
    Task<Emprestimo> CriarAsync(Emprestimo emprestimo);
    Task<bool> AtualizarAsync(string id, Emprestimo emprestimo);
    Task<bool> DeletarAsync(string id);
}
