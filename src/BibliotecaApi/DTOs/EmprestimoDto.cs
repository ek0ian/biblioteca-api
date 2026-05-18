using System.ComponentModel.DataAnnotations;
using BibliotecaApi.Models;

namespace BibliotecaApi.DTOs;

public class CriarEmprestimoDto
{
    [Required(ErrorMessage = "LivroId é obrigatório")]
    public string LivroId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data de devolução prevista é obrigatória")]
    public DateTime DataDevolucaoPrevista { get; set; }
}

public class DevolverLivroDto
{
    public DateTime? DataDevolucaoReal { get; set; }
}

public class EmprestimoResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string LivroId { get; set; } = string.Empty;
    public string TituloLivro { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoReal { get; set; }
    public StatusEmprestimo Status { get; set; }
}
