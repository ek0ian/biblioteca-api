using BibliotecaApi.Models;
using BibliotecaApi.Repositories.Interfaces;
using BibliotecaApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BibliotecaApi.Repositories;

public class LivroRepository : ILivroRepository
{
    private readonly IMongoCollection<Livro> _livros;

    public LivroRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _livros = database.GetCollection<Livro>(settings.Value.LivrosCollection);
    }

    public async Task<List<Livro>> ObterTodosAsync() =>
        await _livros.Find(_ => true).ToListAsync();

    public async Task<Livro?> ObterPorIdAsync(string id) =>
        await _livros.Find(l => l.Id == id).FirstOrDefaultAsync();

    public async Task<Livro?> ObterPorIsbnAsync(string isbn) =>
        await _livros.Find(l => l.Isbn == isbn).FirstOrDefaultAsync();

    public async Task<Livro> CriarAsync(Livro livro)
    {
        await _livros.InsertOneAsync(livro);
        return livro;
    }

    public async Task<bool> AtualizarAsync(string id, Livro livro)
    {
        var result = await _livros.ReplaceOneAsync(l => l.Id == id, livro);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var result = await _livros.DeleteOneAsync(l => l.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<bool> AtualizarQuantidadeAsync(string id, int delta)
    {
        var update = Builders<Livro>.Update.Inc(l => l.QuantidadeDisponivel, delta);
        var result = await _livros.UpdateOneAsync(l => l.Id == id, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
