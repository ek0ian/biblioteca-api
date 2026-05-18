using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using BibliotecaApi.Repositories.Interfaces;
using BibliotecaApi.Services.Interfaces;

namespace BibliotecaApi.Services;

public class EmprestimoService : IEmprestimoService
{
    private readonly IEmprestimoRepository _emprestimoRepository;
    private readonly ILivroRepository _livroRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public EmprestimoService(
        IEmprestimoRepository emprestimoRepository,
        ILivroRepository livroRepository,
        IUsuarioRepository usuarioRepository)
    {
        _emprestimoRepository = emprestimoRepository;
        _livroRepository = livroRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<EmprestimoResponseDto>> ObterTodosAsync()
    {
        var emprestimos = await _emprestimoRepository.ObterTodosAsync();
        return emprestimos.Select(MapearParaDto).ToList();
    }

    public async Task<List<EmprestimoResponseDto>> ObterMeusEmprestimosAsync(string usuarioId)
    {
        var emprestimos = await _emprestimoRepository.ObterPorUsuarioAsync(usuarioId);
        return emprestimos.Select(MapearParaDto).ToList();
    }

    public async Task<EmprestimoResponseDto> ObterPorIdAsync(string id)
    {
        var emprestimo = await _emprestimoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Empréstimo com id '{id}' não encontrado.");
        return MapearParaDto(emprestimo);
    }

    public async Task<EmprestimoResponseDto> CriarAsync(CriarEmprestimoDto dto, string usuarioId)
    {
        var livro = await _livroRepository.ObterPorIdAsync(dto.LivroId)
            ?? throw new KeyNotFoundException($"Livro com id '{dto.LivroId}' não encontrado.");

        if (livro.QuantidadeDisponivel <= 0)
            throw new InvalidOperationException("Não há exemplares disponíveis para empréstimo.");

        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var emprestimo = new Emprestimo
        {
            LivroId = dto.LivroId,
            UsuarioId = usuarioId,
            NomeUsuario = usuario.Nome,
            TituloLivro = livro.Titulo,
            DataEmprestimo = DateTime.UtcNow,
            DataDevolucaoPrevista = dto.DataDevolucaoPrevista,
            Status = StatusEmprestimo.Ativo
        };

        await _livroRepository.AtualizarQuantidadeAsync(dto.LivroId, -1);
        var criado = await _emprestimoRepository.CriarAsync(emprestimo);
        return MapearParaDto(criado);
    }

    public async Task<EmprestimoResponseDto> DevolverAsync(string id, DevolverLivroDto dto)
    {
        var emprestimo = await _emprestimoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Empréstimo com id '{id}' não encontrado.");

        if (emprestimo.Status == StatusEmprestimo.Devolvido)
            throw new InvalidOperationException("Este empréstimo já foi devolvido.");

        emprestimo.DataDevolucaoReal = dto.DataDevolucaoReal ?? DateTime.UtcNow;
        emprestimo.Status = StatusEmprestimo.Devolvido;

        await _emprestimoRepository.AtualizarAsync(id, emprestimo);
        await _livroRepository.AtualizarQuantidadeAsync(emprestimo.LivroId, 1);

        return MapearParaDto(emprestimo);
    }

    public async Task DeletarAsync(string id)
    {
        var emprestimo = await _emprestimoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Empréstimo com id '{id}' não encontrado.");

        if (emprestimo.Status == StatusEmprestimo.Ativo)
            await _livroRepository.AtualizarQuantidadeAsync(emprestimo.LivroId, 1);

        await _emprestimoRepository.DeletarAsync(id);
    }

    private static EmprestimoResponseDto MapearParaDto(Emprestimo e) => new()
    {
        Id = e.Id!,
        LivroId = e.LivroId,
        TituloLivro = e.TituloLivro,
        UsuarioId = e.UsuarioId,
        NomeUsuario = e.NomeUsuario,
        DataEmprestimo = e.DataEmprestimo,
        DataDevolucaoPrevista = e.DataDevolucaoPrevista,
        DataDevolucaoReal = e.DataDevolucaoReal,
        Status = e.Status
    };
}
