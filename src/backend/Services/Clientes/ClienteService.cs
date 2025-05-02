using backend.Models;
using backend.Repository;
using backend.Validacao;

namespace backend.Services.Clientes;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Cliente>> ObterTodosAsync()
        => await _repository.ObterTodosAsync();

    public async Task<Cliente> ObterPorIdAsync(Guid id)
    {
        var cliente = await _repository.ObterPorIdAsync(id);
        if (cliente is null)
            throw new InvalidOperationException("Cliente não encontrado.");

        return cliente;
    }

    public async Task AdicionarAsync(Cliente cliente)
    {
        await _repository.AdicionarAsync(cliente);
    }

    public async Task AtualizarAsync(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nome))
            throw new ArgumentException("Nome é obrigatório.");

        await _repository.AtualizarAsync(cliente);
    }

    public async Task RemoverAsync(Guid id)
        => await _repository.RemoverAsync(id);
}
