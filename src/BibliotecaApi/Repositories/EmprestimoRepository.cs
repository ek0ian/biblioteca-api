using BibliotecaApi.Models;
using BibliotecaApi.Repositories.Interfaces;
using BibliotecaApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BibliotecaApi.Repositories;

public class EmprestimoRepository : IEmprestimoRepository
{
    private readonly IMongoCollection<Emprestimo> _emprestimos;

    public EmprestimoRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _emprestimos = database.GetCollection<Emprestimo>(settings.Value.EmprestimosCollection);
    }

    public async Task<List<Emprestimo>> ObterTodosAsync() =>
        await _emprestimos.Find(_ => true).SortByDescending(e => e.DataEmprestimo).ToListAsync();

    public async Task<List<Emprestimo>> ObterPorUsuarioAsync(string usuarioId) =>
        await _emprestimos.Find(e => e.UsuarioId == usuarioId).ToListAsync();

    public async Task<Emprestimo?> ObterPorIdAsync(string id) =>
        await _emprestimos.Find(e => e.Id == id).FirstOrDefaultAsync();

    public async Task<Emprestimo> CriarAsync(Emprestimo emprestimo)
    {
        await _emprestimos.InsertOneAsync(emprestimo);
        return emprestimo;
    }

    public async Task<bool> AtualizarAsync(string id, Emprestimo emprestimo)
    {
        var result = await _emprestimos.ReplaceOneAsync(e => e.Id == id, emprestimo);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var result = await _emprestimos.DeleteOneAsync(e => e.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}
