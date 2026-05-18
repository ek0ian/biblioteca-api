namespace BibliotecaApi.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "biblioteca";
    public string LivrosCollection { get; set; } = "livros";
    public string EmprestimosCollection { get; set; } = "emprestimos";
    public string UsuariosCollection { get; set; } = "usuarios";
}
