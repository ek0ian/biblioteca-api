using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using BibliotecaApi.Repositories.Interfaces;
using BibliotecaApi.Services.Interfaces;

namespace BibliotecaApi.Services;

public class LivroService : ILivroService
{
    private readonly ILivroRepository _livroRepository;

    public LivroService(ILivroRepository livroRepository)
    {
        _livroRepository = livroRepository;
    }

    public async Task<List<LivroResponseDto>> ObterTodosAsync()
    {
        var livros = await _livroRepository.ObterTodosAsync();
        return livros.Select(MapearParaDto).ToList();
    }

    public async Task<LivroResponseDto> ObterPorIdAsync(string id)
    {
        var livro = await _livroRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Livro com id '{id}' não encontrado.");
        return MapearParaDto(livro);
    }

    public async Task<LivroResponseDto> CriarAsync(CriarLivroDto dto)
    {
        var existente = await _livroRepository.ObterPorIsbnAsync(dto.Isbn);
        if (existente != null)
            throw new InvalidOperationException($"Já existe um livro com o ISBN '{dto.Isbn}'.");

        var livro = new Livro
        {
            Titulo = dto.Titulo,
            Autor = dto.Autor,
            Isbn = dto.Isbn,
            AnoPublicacao = dto.AnoPublicacao,
            Genero = dto.Genero,
            QuantidadeTotal = dto.QuantidadeTotal,
            QuantidadeDisponivel = dto.QuantidadeTotal
        };

        var criado = await _livroRepository.CriarAsync(livro);
        return MapearParaDto(criado);
    }

    public async Task<LivroResponseDto> AtualizarAsync(string id, AtualizarLivroDto dto)
    {
        var livro = await _livroRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Livro com id '{id}' não encontrado.");

        if (dto.Titulo != null) livro.Titulo = dto.Titulo;
        if (dto.Autor != null) livro.Autor = dto.Autor;
        if (dto.Genero != null) livro.Genero = dto.Genero;
        if (dto.AnoPublicacao.HasValue) livro.AnoPublicacao = dto.AnoPublicacao.Value;
        if (dto.QuantidadeTotal.HasValue)
        {
            var diferenca = dto.QuantidadeTotal.Value - livro.QuantidadeTotal;
            livro.QuantidadeTotal = dto.QuantidadeTotal.Value;
            livro.QuantidadeDisponivel = Math.Max(0, livro.QuantidadeDisponivel + diferenca);
        }
        if (dto.Isbn != null) livro.Isbn = dto.Isbn;

        await _livroRepository.AtualizarAsync(id, livro);
        return MapearParaDto(livro);
    }

    public async Task DeletarAsync(string id)
    {
        var livro = await _livroRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException($"Livro com id '{id}' não encontrado.");

        await _livroRepository.DeletarAsync(livro.Id!);
    }

    private static LivroResponseDto MapearParaDto(Livro livro) => new()
    {
        Id = livro.Id!,
        Titulo = livro.Titulo,
        Autor = livro.Autor,
        Isbn = livro.Isbn,
        AnoPublicacao = livro.AnoPublicacao,
        Genero = livro.Genero,
        QuantidadeTotal = livro.QuantidadeTotal,
        QuantidadeDisponivel = livro.QuantidadeDisponivel,
        CriadoEm = livro.CriadoEm
    };
}
