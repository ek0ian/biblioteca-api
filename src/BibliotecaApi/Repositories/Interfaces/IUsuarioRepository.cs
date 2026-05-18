using BibliotecaApi.Models;

namespace BibliotecaApi.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterPorIdAsync(string id);
    Task<bool> EmailExisteAsync(string email);
    Task<Usuario> CriarAsync(Usuario usuario);
}
