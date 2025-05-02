using backend.Models;
using backend.Repository;
using backend.Validacao;

namespace backend.Services.Produtos;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<IEnumerable<Produto>> ObterTodosAsync()
        => await _produtoRepository.ObterTodosAsync();

    public async Task<Produto?> ObterPorIdAsync(Guid id)
        => await _produtoRepository.ObterPorIdAsync(id);

    public async Task AdicionarProdutoAsync(Produto produto)
    {
        var validador = new ValidarProdutos(); // TODO Fazer a injecao
        var resultado = await validador.ValidateAsync(produto);

        if(!resultado.IsValid)
          throw new ArgumentException(resultado.Errors.First().ErrorMessage);

        await _produtoRepository.AdicionarAsync(produto);
    }

    public async Task AtualizarProdutoAsync(Produto produto)
    {
      var validador = new ValidarProdutos(); // TODO fazer a injecao
      var resultado = await validador.ValidateAsync(produto);

      if(!resultado.IsValid)
        throw new ArgumentException(resultado.Errors.First().ErrorMessage);

      Produto? verificaProduto = await ObterPorIdAsync(produto.Id);
      if(ProdutoIgual(produto,verificaProduto)) // regra de negocio especifica
        throw new ArgumentException("voce nao pode atualizar o produto sem modificacoes");

      await _produtoRepository.AtualizarAsync(produto);
    }

    public async Task RemoverProdutoAsync(Guid id)
    {
      Produto? verificaProduto = await ObterPorIdAsync(id);
      if(verificaProduto is null)
        throw new ArgumentException("Voce nao pode deletar um produto nao criado");

      await _produtoRepository.RemoverAsync(id);
    }

    private bool ProdutoIgual(Produto produto1, Produto produto2)
    {
      return produto1.Nome == produto2.Nome &&
        produto1.Preco == produto2.Preco && // talvez aqui de problemas no futuro
        produto1.QuantidadeDisponivel == produto2.QuantidadeDisponivel &&
        produto1.Descricao == produto2.Descricao &&
        produto1.DataValidade == produto2.DataValidade;
    }
}
