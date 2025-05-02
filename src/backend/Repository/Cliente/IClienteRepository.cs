using backend.Models;

namespace backend.Repository;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObterTodosAsync();
    Task<Cliente?> ObterIdAsync(Guid id);
    Task<Cliente?> ObterEmailAsync(string email);
    Task<Cliente?> ObterCpfCnpjAsync(string cpfCnpj);
    Task AdicionarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
    Task RemoverAsync(Guid id);
}
