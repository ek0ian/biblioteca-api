using BibliotecaApi.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BibliotecaApi.Infrastructure;

public class DatabaseInitializer
{
    private readonly IMongoDatabase _database;

    public DatabaseInitializer(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public async Task InicializarAsync()
    {
        await CriarIndicesLivrosAsync();
        await CriarIndicesEmprestimosAsync();
        await CriarIndicesUsuariosAsync();
    }

    private async Task CriarIndicesLivrosAsync()
    {
        var collection = _database.GetCollection<MongoDB.Bson.BsonDocument>("livros");

        var indices = new[]
        {
            new CreateIndexModel<MongoDB.Bson.BsonDocument>(
                Builders<MongoDB.Bson.BsonDocument>.IndexKeys.Ascending("isbn"),
                new CreateIndexOptions { Unique = true, Name = "idx_livros_isbn_unico" }),

            new CreateIndexModel<MongoDB.Bson.BsonDocument>(
                Builders<MongoDB.Bson.BsonDocument>.IndexKeys.Ascending("titulo"),
                new CreateIndexOptions { Name = "idx_livros_titulo" })
        };

        await collection.Indexes.CreateManyAsync(indices);
    }

    private async Task CriarIndicesEmprestimosAsync()
    {
        var collection = _database.GetCollection<MongoDB.Bson.BsonDocument>("emprestimos");

        var indices = new[]
        {
            new CreateIndexModel<MongoDB.Bson.BsonDocument>(
                Builders<MongoDB.Bson.BsonDocument>.IndexKeys.Ascending("usuarioId"),
                new CreateIndexOptions { Name = "idx_emprestimos_usuario" }),

            new CreateIndexModel<MongoDB.Bson.BsonDocument>(
                Builders<MongoDB.Bson.BsonDocument>.IndexKeys.Ascending("status"),
                new CreateIndexOptions { Name = "idx_emprestimos_status" }),

            new CreateIndexModel<MongoDB.Bson.BsonDocument>(
                Builders<MongoDB.Bson.BsonDocument>.IndexKeys.Ascending("livroId"),
                new CreateIndexOptions { Name = "idx_emprestimos_livro" })
        };

        await collection.Indexes.CreateManyAsync(indices);
    }

    private async Task CriarIndicesUsuariosAsync()
    {
        var collection = _database.GetCollection<MongoDB.Bson.BsonDocument>("usuarios");

        var indice = new CreateIndexModel<MongoDB.Bson.BsonDocument>(
            Builders<MongoDB.Bson.BsonDocument>.IndexKeys.Ascending("email"),
            new CreateIndexOptions { Unique = true, Name = "idx_usuarios_email_unico" });

        await collection.Indexes.CreateOneAsync(indice);
    }
}
