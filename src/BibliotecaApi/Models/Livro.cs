using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BibliotecaApi.Models;

public class Livro
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [BsonElement("autor")]
    public string Autor { get; set; } = string.Empty;

    [BsonElement("isbn")]
    public string Isbn { get; set; } = string.Empty;

    [BsonElement("anoPublicacao")]
    public int AnoPublicacao { get; set; }

    [BsonElement("genero")]
    public string Genero { get; set; } = string.Empty;

    [BsonElement("quantidadeTotal")]
    public int QuantidadeTotal { get; set; }

    [BsonElement("quantidadeDisponivel")]
    public int QuantidadeDisponivel { get; set; }

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
