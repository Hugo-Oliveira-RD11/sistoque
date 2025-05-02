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

    public async Task<IEnumerable<Cliente>?> ObterTodosAsync()
        => await _repository.ObterTodosAsync();

    public async Task<Cliente?> ObterPorIdAsync(Guid id)
    {
        var cliente = await _repository.ObterIdAsync(id);
        if (cliente is null)
          return null;

        return cliente;
    }

    public async Task AdicionarAsync(Cliente cliente)
    {
        var validador = new ValidarCliente(); // TODO fazer a injecao
        var resultado = await validador.ValidateAsync(cliente);
        if(!resultado.IsValid)
          throw new ArgumentException(resultado.Errors.First().ErrorMessage);

        Cliente?  verificaClienteCpfCnpj = await  ObterPorCpfCnpjAsync(cliente.CpfCnpj!);
        if(verificaClienteCpfCnpj != null)
          throw new ArgumentException("Este cpf/cnpj ja foi cadastrado");

        Cliente?  verificaClienteEmail = await ObterPorEmailAsync(cliente.Email!); // cirando regras de negocio
        if(verificaClienteEmail != null)
          throw new ArgumentException("Este email ja foi cadastrado");


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

    public async Task<Cliente?> ObterPorEmailAsync(string email)
    {
      var cliente = await _repository.ObterEmailAsync(email);
      if(cliente is null)
        return null;

      return cliente;
    }

    public async Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj)
    {
      var cliente = await _repository.ObterCpfCnpjAsync(cpfCnpj);
      if(cliente is null)
        return null;

      return cliente;
    }
}
