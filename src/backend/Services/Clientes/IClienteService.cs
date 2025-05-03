using backend.Models;

namespace backend.Services.Clientes;

public interface IClienteService
{
    Task<IEnumerable<Cliente>?> ObterTodosAsync();
    Task<Cliente?> ObterPorIdAsync(Guid id);
    Task<Cliente?> ObterPorEmailAsync(string email);
    Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj);
    Task AdicionarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
    Task RemoverIdAsync(Guid id);
}
