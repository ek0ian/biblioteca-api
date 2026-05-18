using System.ComponentModel.DataAnnotations;

namespace BibliotecaApi.DTOs;

public class CriarLivroDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Autor é obrigatório")]
    public string Autor { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN é obrigatório")]
    public string Isbn { get; set; } = string.Empty;

    [Range(1000, 2100, ErrorMessage = "Ano de publicação inválido")]
    public int AnoPublicacao { get; set; }

    public string Genero { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public int QuantidadeTotal { get; set; }
}

public class AtualizarLivroDto
{
    public string? Titulo { get; set; }
    public string? Autor { get; set; }
    public string? Isbn { get; set; }
    public int? AnoPublicacao { get; set; }
    public string? Genero { get; set; }
    public int? QuantidadeTotal { get; set; }
}

public class LivroResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public int AnoPublicacao { get; set; }
    public string Genero { get; set; } = string.Empty;
    public int QuantidadeTotal { get; set; }
    public int QuantidadeDisponivel { get; set; }
    public DateTime CriadoEm { get; set; }
}
