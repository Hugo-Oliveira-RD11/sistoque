using backend.Models;
using backend.Repository;

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
        if (string.IsNullOrEmpty(produto.Nome))
            throw new ArgumentException("Nome do produto é obrigatório.");

        if(produto.Preco < 0)
            throw new ArgumentException("nao e permitido criar um produto com valor negativo");

        if(produto.QuantidadeDisponivel < 0) // as vezes o produto e digital ou ta em falta no estoque
            throw new ArgumentException("nao e permitido criar um produto com quantidade negativo");

        await _produtoRepository.AdicionarAsync(produto);
    }

    public async Task AtualizarProdutoAsync(Produto produto)
    {
      Produto? verificaProduto = await ObterPorIdAsync(produto.Id);
      if(ProdutoIgual(produto,verificaProduto))
        throw new ArgumentException("voce nao pode atualizar o produto sem modificacoes");

      await _produtoRepository.AtualizarAsync(produto);
    }

    public async Task RemoverProdutoAsync(Guid id)
    {
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
