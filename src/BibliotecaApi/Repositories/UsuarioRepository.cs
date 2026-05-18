using BibliotecaApi.Models;
using BibliotecaApi.Repositories.Interfaces;
using BibliotecaApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BibliotecaApi.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IMongoCollection<Usuario> _usuarios;

    public UsuarioRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _usuarios = database.GetCollection<Usuario>(settings.Value.UsuariosCollection);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email) =>
        await _usuarios.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<Usuario?> ObterPorIdAsync(string id) =>
        await _usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<bool> EmailExisteAsync(string email) =>
        await _usuarios.Find(u => u.Email == email).AnyAsync();

    public async Task<Usuario> CriarAsync(Usuario usuario)
    {
        await _usuarios.InsertOneAsync(usuario);
        return usuario;
    }
}
