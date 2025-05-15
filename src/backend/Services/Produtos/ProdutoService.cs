using System.Security.Claims;
using backend.Models;
using backend.Repository.Produtos;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace backend.Services.Produtos;

public class ProdutoService : IProdutoService
{
  private readonly IProdutoRepository _produtoRepository;
  private readonly IValidator<Produto> _validator;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public ProdutoService(
    IProdutoRepository produtoRepository,
    IValidator<Produto> validator,
    IHttpContextAccessor httpContextAccessor)
  {
    _produtoRepository = produtoRepository;
    _validator = validator;
    _httpContextAccessor = httpContextAccessor;
  }

  private Guid GetUsuarioId()
  {
    // Obtém o ID do usuário do token JWT
    var userId = _httpContextAccessor.HttpContext?.User?
      .FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var usuarioId))
      throw new UnauthorizedAccessException("Usuário não autenticado");

    return usuarioId;
  }

  public async Task<IEnumerable<Produto>> ObterTodosAsync()
  {
    var usuarioId = GetUsuarioId();
    return await _produtoRepository.ObterPorUsuarioIdAsync(usuarioId);
  }

  public async Task<Produto?> ObterPorIdAsync(Guid id)
  {
    var usuarioId = GetUsuarioId();
    var produto = await _produtoRepository.ObterPorIdAsync(id);

    if (produto == null || produto.usuarioId != usuarioId)
      throw new UnauthorizedAccessException("Produto não encontrado ou acesso negado");

    return produto;
  }

  public async Task AdicionarProdutoAsync(Produto produto)
  {
      if (produto == null)
        throw new ArgumentNullException(nameof(produto), "O produto não pode ser nulo");

      var usuarioId = GetUsuarioId();
      produto.usuarioId = usuarioId;

      var validationResult = await _validator.ValidateAsync(produto);
      if (!validationResult.IsValid)
        throw new ValidationException(validationResult.Errors.First().ErrorMessage);

      await _produtoRepository.AdicionarAsync(produto);
    }

  public async Task AtualizarProdutoAsync(Produto produto)
  {
    var usuarioId = GetUsuarioId();
    var produtoExistente = await _produtoRepository.ObterPorIdAsync(produto.Id);

    if (produtoExistente == null || produtoExistente.usuarioId != usuarioId)
    {
      throw new UnauthorizedAccessException("Produto não encontrado ou acesso negado");
    }
    var validationResult =  await _validator.ValidateAsync(produto);
    if(!validationResult.IsValid)
      throw new ValidationException(validationResult.Errors.First().ErrorMessage);

    produto.usuarioId = usuarioId;
    Produto? verificaProduto = await ObterPorIdAsync(produto.Id);
    if(ProdutoIgual(produto,verificaProduto))
      throw new ArgumentException("voce nao pode atualizar o produto sem modificacoes");

    await _produtoRepository.AtualizarAsync(produto);
  }

  public async Task RemoverProdutoAsync(Guid id)
  {
    var usuarioId = GetUsuarioId();
    var produto = await _produtoRepository.ObterPorIdAsync(id);

    if (produto == null || produto.usuarioId != usuarioId)
    {
      throw new UnauthorizedAccessException("Produto não encontrado ou acesso negado");
    }

    await _produtoRepository.RemoverAsync(id);
  }
  private static bool ProdutoIgual(Produto produto1, Produto produto2)
  {
    if (produto1 == null || produto2 == null)
      return false;

    if (ReferenceEquals(produto1, produto2))
      return true;

    return produto1.Id == produto2.Id &&
           string.Equals(produto1.Nome, produto2.Nome, StringComparison.OrdinalIgnoreCase) &&
           decimal.Equals(produto1.Preco, produto2.Preco) && // Precisão decimal segura
           produto1.QuantidadeDisponivel == produto2.QuantidadeDisponivel &&
           string.Equals(produto1.Descricao, produto2.Descricao, StringComparison.Ordinal) &&
           produto1.DataValidade == produto2.DataValidade;
  }
}
