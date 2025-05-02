using backend.Models;

namespace backend.Services.Clientes;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ObterTodosAsync();
    Task<Cliente> ObterPorIdAsync(Guid id);
    Task AdicionarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
    Task RemoverAsync(Guid id);
}
