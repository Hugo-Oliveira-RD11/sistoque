using backend.Models;

namespace backend.Services.Produtos;

public interface IProdutoService
{
    Task<IEnumerable<Produto>> ObterTodosAsync();
    Task<Produto?> ObterPorIdAsync(Guid id);
    Task AdicionarProdutoAsync(Produto produto);
    Task AtualizarProdutoAsync(Produto produto);
    Task RemoverProdutoAsync(Guid id);
}
