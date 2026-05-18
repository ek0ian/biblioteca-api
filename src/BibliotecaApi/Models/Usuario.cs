using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BibliotecaApi.Models;

public enum PerfilUsuario
{
    Usuario,
    Admin
}

public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("senhaHash")]
    public string SenhaHash { get; set; } = string.Empty;

    [BsonElement("perfil")]
    [BsonRepresentation(BsonType.String)]
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Usuario;

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
