using backend.Models;
using backend.Repository;
using FluentValidation;

namespace backend.Services.Clientes;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly IValidator<Cliente> _validator;

    public ClienteService(IClienteRepository repository, IValidator<Cliente> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<Cliente>?> ObterTodosAsync()
        => await _repository.ObterTodosAsync();

    public async Task<Cliente?> ObterPorIdAsync(Guid id)
    {
        var cliente = await _repository.ObterIdAsync(id);
        return cliente;
    }

    public async Task AdicionarAsync(Cliente cliente)
    {
        var resultado = await _validator.ValidateAsync(cliente);
        if (!resultado.IsValid)
            throw new ArgumentException(resultado.Errors.First().ErrorMessage);

        Cliente? verificaClienteCpfCnpj = await ObterPorCpfCnpjAsync(cliente.CpfCnpj!);
        if (verificaClienteCpfCnpj != null)
            throw new ArgumentException("Este cpf/cnpj já foi cadastrado");

        Cliente? verificaClienteEmail = await ObterPorEmailAsync(cliente.Email!);
        if (verificaClienteEmail != null)
            throw new ArgumentException("Este email já foi cadastrado");

        await _repository.AdicionarAsync(cliente);
    }

    public async Task AtualizarAsync(Cliente cliente)
    {
        var resultado = await _validator.ValidateAsync(cliente);
        if (!resultado.IsValid)
            throw new ArgumentException(resultado.Errors.First().ErrorMessage);

        Cliente? clienteVerifica = null;

        if (cliente.Id != Guid.Empty)
            clienteVerifica = await ObterPorIdAsync(cliente.Id);
        else if (!string.IsNullOrEmpty(cliente.Email))
            clienteVerifica = await ObterPorEmailAsync(cliente.Email);
        else if (!string.IsNullOrEmpty(cliente.CpfCnpj))
            clienteVerifica = await ObterPorCpfCnpjAsync(cliente.CpfCnpj);

        if (clienteVerifica == null)
            throw new ArgumentException("Usuário não existe/email incorreto");

        if (ClienteIgual(cliente, clienteVerifica))
            throw new ArgumentException("Não se pode atualizar sem mudar alguma informação");

        await _repository.AtualizarAsync(cliente);
    }

    public async Task RemoverIdAsync(Guid id)
    {
        var clienteVerifica = await ObterPorIdAsync(id);
        if (clienteVerifica == null)
            throw new InvalidOperationException("Não pode remover um usuário que não existe");

        if (clienteVerifica.Role != "admin")
            throw new InvalidOperationException("Usuário sem ser admin não pode remover usuário");

        await _repository.RemoverAsync(id);
    }

    public async Task<Cliente?> ObterPorEmailAsync(string email)
        => await _repository.ObterEmailAsync(email);

    public async Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj)
        => await _repository.ObterCpfCnpjAsync(cpfCnpj);

    private bool ClienteIgual(Cliente cliente1, Cliente cliente2)
    {
        return string.Equals(cliente1.Nome, cliente2.Nome) &&
               string.Equals(cliente1.CpfCnpj, cliente2.CpfCnpj) &&
               string.Equals(cliente1.Telefone, cliente2.Telefone) &&
               string.Equals(cliente1.Email, cliente2.Email) &&
               string.Equals(cliente1.Role, cliente2.Role);
    }
}
