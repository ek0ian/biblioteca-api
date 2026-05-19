using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using BibliotecaApi.Repositories.Interfaces;
using BibliotecaApi.Services;
using Moq;

namespace BibliotecaApi.Tests;

public class LivroServiceTests
{
    private readonly Mock<ILivroRepository> _repositoryMock;
    private readonly LivroService _service;

    public LivroServiceTests()
    {
        _repositoryMock = new Mock<ILivroRepository>();
        _service = new LivroService(_repositoryMock.Object);
    }

    // --- Cenários de sucesso ---

    [Fact]
    public async Task CriarAsync_ComDadosValidos_RetornaLivroCriado()
    {
        var dto = new CriarLivroDto
        {
            Titulo = "Clean Code",
            Autor = "Robert C. Martin",
            Isbn = "978-0132350884",
            AnoPublicacao = 2008,
            Genero = "Tecnologia",
            QuantidadeTotal = 3
        };

        _repositoryMock.Setup(r => r.ObterPorIsbnAsync(dto.Isbn)).ReturnsAsync((Livro?)null);
        _repositoryMock.Setup(r => r.CriarAsync(It.IsAny<Livro>()))
            .ReturnsAsync((Livro l) => { l.Id = "abc123"; return l; });

        var resultado = await _service.CriarAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal(dto.Titulo, resultado.Titulo);
        Assert.Equal(dto.QuantidadeTotal, resultado.QuantidadeDisponivel);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdExistente_RetornaLivro()
    {
        var livro = new Livro
        {
            Id = "abc123",
            Titulo = "Domain-Driven Design",
            Autor = "Eric Evans",
            Isbn = "978-0321125217",
            AnoPublicacao = 2003,
            Genero = "Tecnologia",
            QuantidadeTotal = 2,
            QuantidadeDisponivel = 2
        };

        _repositoryMock.Setup(r => r.ObterPorIdAsync("abc123")).ReturnsAsync(livro);

        var resultado = await _service.ObterPorIdAsync("abc123");

        Assert.NotNull(resultado);
        Assert.Equal("Domain-Driven Design", resultado.Titulo);
        Assert.Equal("abc123", resultado.Id);
    }

    [Fact]
    public async Task ObterTodosAsync_RetornaListaDeLivros()
    {
        var livros = new List<Livro>
        {
            new() { Id = "1", Titulo = "Livro A", Autor = "Autor A", Isbn = "111", AnoPublicacao = 2020, Genero = "Ficção", QuantidadeTotal = 1, QuantidadeDisponivel = 1 },
            new() { Id = "2", Titulo = "Livro B", Autor = "Autor B", Isbn = "222", AnoPublicacao = 2021, Genero = "Romance", QuantidadeTotal = 2, QuantidadeDisponivel = 2 }
        };

        _repositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(livros);

        var resultado = await _service.ObterTodosAsync();

        Assert.Equal(2, resultado.Count);
    }

    // --- Cenários de erro ---

    [Fact]
    public async Task CriarAsync_ComIsbnDuplicado_LancaInvalidOperationException()
    {
        var dto = new CriarLivroDto
        {
            Titulo = "Clean Code",
            Autor = "Robert C. Martin",
            Isbn = "978-0132350884",
            AnoPublicacao = 2008,
            Genero = "Tecnologia",
            QuantidadeTotal = 1
        };

        _repositoryMock.Setup(r => r.ObterPorIsbnAsync(dto.Isbn))
            .ReturnsAsync(new Livro { Id = "existente", Isbn = dto.Isbn });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_LancaKeyNotFoundException()
    {
        _repositoryMock.Setup(r => r.ObterPorIdAsync("nao-existe")).ReturnsAsync((Livro?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ObterPorIdAsync("nao-existe"));
    }

    [Fact]
    public async Task DeletarAsync_ComIdInexistente_LancaKeyNotFoundException()
    {
        _repositoryMock.Setup(r => r.ObterPorIdAsync("xyz")).ReturnsAsync((Livro?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeletarAsync("xyz"));
    }
}
