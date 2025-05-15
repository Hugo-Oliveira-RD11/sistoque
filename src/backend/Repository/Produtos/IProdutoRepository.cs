using backend.Models;

namespace backend.Repository.Produtos;

public interface IProdutoRepository
{
  Task<IEnumerable<Produto>> ObterTodosAsync();
  Task<IEnumerable<Produto>> ObterPorUsuarioIdAsync(Guid usuarioId);
  Task<Produto?> ObterPorIdAsync(Guid id);
  Task<Produto?> ObterPorIdEUsuarioIdAsync(Guid id, Guid usuarioId);
  Task AdicionarAsync(Produto produto);
  Task AtualizarAsync(Produto produto);
  Task RemoverAsync(Guid id);
  Task RemoverPorIdEUsuarioIdAsync(Guid id, Guid usuarioId);
}
