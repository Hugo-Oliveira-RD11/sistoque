using backend.Models;

namespace backend.Repository;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> ObterTodosAsync();
    Task<Produto?> ObterPorIdAsync(Guid id);
    Task AdicionarAsync(Produto produto);
    Task AtualizarAsync(Produto produto);
    Task RemoverAsync(Guid id);
}
