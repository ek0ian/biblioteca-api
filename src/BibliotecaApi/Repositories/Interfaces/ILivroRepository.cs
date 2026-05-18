using BibliotecaApi.Models;

namespace BibliotecaApi.Repositories.Interfaces;

public interface ILivroRepository
{
    Task<List<Livro>> ObterTodosAsync();
    Task<Livro?> ObterPorIdAsync(string id);
    Task<Livro?> ObterPorIsbnAsync(string isbn);
    Task<Livro> CriarAsync(Livro livro);
    Task<bool> AtualizarAsync(string id, Livro livro);
    Task<bool> DeletarAsync(string id);
    Task<bool> AtualizarQuantidadeAsync(string id, int delta);
}
