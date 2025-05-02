using backend.Models;

namespace backend.Repository;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObterTodosAsync();
    Task<Cliente?> ObterPorIdAsync(Guid id);
    Task<Cliente?> ObterPorEmailAsync(string email);
    Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj);
    Task AdicionarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
    Task RemoverAsync(Guid id);
}
