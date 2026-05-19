using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using BibliotecaApi.Repositories.Interfaces;
using BibliotecaApi.Services;
using Moq;

namespace BibliotecaApi.Tests;

public class EmprestimoServiceTests
{
    private readonly Mock<IEmprestimoRepository> _emprestimoRepoMock;
    private readonly Mock<ILivroRepository> _livroRepoMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly EmprestimoService _service;

    public EmprestimoServiceTests()
    {
        _emprestimoRepoMock = new Mock<IEmprestimoRepository>();
        _livroRepoMock = new Mock<ILivroRepository>();
        _usuarioRepoMock = new Mock<IUsuarioRepository>();

        _service = new EmprestimoService(
            _emprestimoRepoMock.Object,
            _livroRepoMock.Object,
            _usuarioRepoMock.Object
        );
    }

    // --- Cenários de sucesso ---

    [Fact]
    public async Task CriarAsync_ComLivroDisponivel_RetornaEmprestimoCriado()
    {
        var livro = new Livro { Id = "livro1", Titulo = "Clean Code", QuantidadeDisponivel = 2 };
        var usuario = new Usuario { Id = "user1", Nome = "João Silva" };
        var dto = new CriarEmprestimoDto
        {
            LivroId = "livro1",
            DataDevolucaoPrevista = DateTime.UtcNow.AddDays(14)
        };

        _livroRepoMock.Setup(r => r.ObterPorIdAsync("livro1")).ReturnsAsync(livro);
        _usuarioRepoMock.Setup(r => r.ObterPorIdAsync("user1")).ReturnsAsync(usuario);
        _livroRepoMock.Setup(r => r.AtualizarQuantidadeAsync("livro1", -1)).ReturnsAsync(true);
        _emprestimoRepoMock.Setup(r => r.CriarAsync(It.IsAny<Emprestimo>()))
            .ReturnsAsync((Emprestimo e) => { e.Id = "emp1"; return e; });

        var resultado = await _service.CriarAsync(dto, "user1");

        Assert.NotNull(resultado);
        Assert.Equal("livro1", resultado.LivroId);
        Assert.Equal(StatusEmprestimo.Ativo, resultado.Status);
        _livroRepoMock.Verify(r => r.AtualizarQuantidadeAsync("livro1", -1), Times.Once);
    }

    [Fact]
    public async Task DevolverAsync_ComEmprestimoAtivo_AtualizaStatusEEstoque()
    {
        var emprestimo = new Emprestimo
        {
            Id = "emp1",
            LivroId = "livro1",
            UsuarioId = "user1",
            Status = StatusEmprestimo.Ativo,
            DataEmprestimo = DateTime.UtcNow.AddDays(-7),
            DataDevolucaoPrevista = DateTime.UtcNow.AddDays(7)
        };

        _emprestimoRepoMock.Setup(r => r.ObterPorIdAsync("emp1")).ReturnsAsync(emprestimo);
        _emprestimoRepoMock.Setup(r => r.AtualizarAsync("emp1", It.IsAny<Emprestimo>())).ReturnsAsync(true);
        _livroRepoMock.Setup(r => r.AtualizarQuantidadeAsync("livro1", 1)).ReturnsAsync(true);

        var resultado = await _service.DevolverAsync("emp1", new DevolverLivroDto());

        Assert.Equal(StatusEmprestimo.Devolvido, resultado.Status);
        Assert.NotNull(resultado.DataDevolucaoReal);
        _livroRepoMock.Verify(r => r.AtualizarQuantidadeAsync("livro1", 1), Times.Once);
    }

    // --- Cenários de erro ---

    [Fact]
    public async Task CriarAsync_ComLivroSemEstoque_LancaInvalidOperationException()
    {
        var livro = new Livro { Id = "livro1", Titulo = "Clean Code", QuantidadeDisponivel = 0 };
        var dto = new CriarEmprestimoDto { LivroId = "livro1", DataDevolucaoPrevista = DateTime.UtcNow.AddDays(14) };

        _livroRepoMock.Setup(r => r.ObterPorIdAsync("livro1")).ReturnsAsync(livro);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto, "user1"));
    }

    [Fact]
    public async Task CriarAsync_ComLivroInexistente_LancaKeyNotFoundException()
    {
        var dto = new CriarEmprestimoDto { LivroId = "nao-existe", DataDevolucaoPrevista = DateTime.UtcNow.AddDays(14) };

        _livroRepoMock.Setup(r => r.ObterPorIdAsync("nao-existe")).ReturnsAsync((Livro?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CriarAsync(dto, "user1"));
    }

    [Fact]
    public async Task DevolverAsync_ComEmprestimoJaDevolvido_LancaInvalidOperationException()
    {
        var emprestimo = new Emprestimo
        {
            Id = "emp1",
            LivroId = "livro1",
            Status = StatusEmprestimo.Devolvido,
            DataEmprestimo = DateTime.UtcNow.AddDays(-14),
            DataDevolucaoPrevista = DateTime.UtcNow
        };

        _emprestimoRepoMock.Setup(r => r.ObterPorIdAsync("emp1")).ReturnsAsync(emprestimo);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DevolverAsync("emp1", new DevolverLivroDto()));
    }
}
