using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BibliotecaApi.Models;

public enum StatusEmprestimo
{
    Ativo,
    Devolvido,
    Atrasado
}

public class Emprestimo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("livroId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string LivroId { get; set; } = string.Empty;

    [BsonElement("usuarioId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UsuarioId { get; set; } = string.Empty;

    [BsonElement("nomeUsuario")]
    public string NomeUsuario { get; set; } = string.Empty;

    [BsonElement("tituloLivro")]
    public string TituloLivro { get; set; } = string.Empty;

    [BsonElement("dataEmprestimo")]
    public DateTime DataEmprestimo { get; set; } = DateTime.UtcNow;

    [BsonElement("dataDevolucaoPrevista")]
    public DateTime DataDevolucaoPrevista { get; set; }

    [BsonElement("dataDevolucaoReal")]
    public DateTime? DataDevolucaoReal { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public StatusEmprestimo Status { get; set; } = StatusEmprestimo.Ativo;
}
