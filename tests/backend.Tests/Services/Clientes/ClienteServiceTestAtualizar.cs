using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using backend.Services.Clientes.Auth;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestAtualizar
{
    private readonly Mock<IClienteRepository> _mockRepo;
    private readonly Mock<ITokenService> _mockToken;
    private readonly Mock<IHashServices> _mockHash;
    private readonly Mock<IValidator<Cliente>> _mockValidator;
    private readonly ClienteService _service;

    public ClienteServiceTestAtualizar()
    {
        _mockRepo = new Mock<IClienteRepository>();
        _mockValidator = new Mock<IValidator<Cliente>>();
        _mockToken = new Mock<ITokenService>();
        _mockHash = new Mock<IHashServices>();
        _service = new ClienteService(_mockRepo.Object, _mockValidator.Object, _mockToken.Object, _mockHash.Object);
    }

    private void SetupValidatorSuccess()
    {
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Cliente>(), default))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidatorFailure(string errorMessage)
    {
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("property", errorMessage)
        };
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Cliente>(), default))
            .ReturnsAsync(new ValidationResult(failures));
    }

    private Cliente CriarClienteValido()
    {
        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Hugo Valido",
            CpfCnpj = "12345678912",
            Telefone = "61987654321",
            Email = "hugo.valido@test.com",
            SenhaHash = "senhaSegura123",
            Role = "user"
        };
    }

    private Cliente CriarClientePadrao(Guid? id = null)
    {
        return new Cliente
        {
            Id = id ?? Guid.Empty,
            Nome = "Cliente Teste",
            Email = "teste@teste.com",
            CpfCnpj = "12345678901",
            Telefone = "11999999999",
            SenhaHash = "123456",
            Role = "user"
        };
    }

    private Cliente CriarClienteComDiferencas(Cliente original)
    {
        return new Cliente
        {
            Id = original.Id,
            Nome = original.Nome + " Modificado",
            Email = original.Email,
            CpfCnpj = original.CpfCnpj,
            Telefone = original.Telefone,
            SenhaHash = original.SenhaHash,
            Role = original.Role
        };
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoValidacaoFalhar()
    {
        // Arrange
        SetupValidatorFailure("Erro de validação");
        var cliente = CriarClienteValido();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(cliente));
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoClienteNaoExistir()
    {
        // Arrange
        SetupValidatorSuccess();
        var cliente = CriarClienteValido();

        _mockRepo.Setup(repo => repo.ObterIdAsync(cliente.Id)).ReturnsAsync((Cliente?)null);
        _mockRepo.Setup(repo => repo.ObterEmailAsync(cliente.Email)).ReturnsAsync((Cliente?)null);
        _mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(cliente.CpfCnpj)).ReturnsAsync((Cliente?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(cliente));
        Assert.Contains("não existe", ex.Message.ToLower());
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoNenhumaMudanca()
    {
        // Arrange
        SetupValidatorSuccess();
        var clienteExistente = CriarClienteValido();
        var clienteAtualizacao = CriarClienteValido(); // Cópia exata

        _mockRepo.Setup(repo => repo.ObterIdAsync(clienteExistente.Id))
               .ReturnsAsync(clienteExistente);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(clienteAtualizacao));
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizar_QuandoClienteExistirEComMudancas()
    {
        // Arrange
        SetupValidatorSuccess();
        var clienteOriginal = CriarClienteValido();
        var clienteModificado = CriarClienteComDiferencas(clienteOriginal);

        _mockRepo.Setup(repo => repo.ObterIdAsync(clienteOriginal.Id))
               .ReturnsAsync(clienteOriginal);

        // Act
        await _service.AtualizarAsync(clienteModificado);

        // Assert
        _mockRepo.Verify(repo => repo.AtualizarAsync(clienteModificado), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveBuscarPorId_QuandoIdForValido()
    {
        // Arrange
        SetupValidatorSuccess();
        var cliente = CriarClienteValido();
        var clienteModificado = CriarClienteComDiferencas(cliente);

        _mockRepo.Setup(repo => repo.ObterIdAsync(cliente.Id))
               .ReturnsAsync(cliente);

        // Act
        await _service.AtualizarAsync(clienteModificado);

        // Assert
        _mockRepo.Verify(repo => repo.ObterIdAsync(cliente.Id), Times.Once);
        _mockRepo.Verify(repo => repo.ObterEmailAsync(It.IsAny<string>()), Times.Never);
        _mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveBuscarPorEmail_QuandoIdForVazio()
    {
        // Arrange
        SetupValidatorSuccess();
        var cliente = CriarClientePadrao(); // Id vazio
        var clienteModificado = CriarClienteComDiferencas(cliente);

        _mockRepo.Setup(repo => repo.ObterEmailAsync(cliente.Email))
               .ReturnsAsync(cliente);

        // Act
        await _service.AtualizarAsync(clienteModificado);

        // Assert
        _mockRepo.Verify(repo => repo.ObterEmailAsync(cliente.Email), Times.Once);
        _mockRepo.Verify(repo => repo.ObterIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveBuscarPorCpfCnpj_QuandoIdForVazioEEmailForVazio()
    {
        // Arrange
        SetupValidatorSuccess();
        var cliente = CriarClientePadrao(); // Id vazio
        cliente.Email = null; // Email vazio
        var clienteModificado = CriarClienteComDiferencas(cliente);

        _mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(cliente.CpfCnpj))
               .ReturnsAsync(cliente);

        // Act
        await _service.AtualizarAsync(clienteModificado);

        // Assert
        _mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(cliente.CpfCnpj), Times.Once);
        _mockRepo.Verify(repo => repo.ObterIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockRepo.Verify(repo => repo.ObterEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveSeguirOrdemCorretaDeBusca_IdEmailCpfCnpj()
    {
        // Arrange
        SetupValidatorSuccess();
        var cliente = CriarClienteValido(); // Com ID válido
        var clienteModificado = CriarClienteComDiferencas(cliente);

        _mockRepo.Setup(repo => repo.ObterIdAsync(cliente.Id))
               .ReturnsAsync(cliente);

        // Act
        await _service.AtualizarAsync(clienteModificado);

        // Assert - Verifica que só chamou ObterPorIdAsync
        _mockRepo.Verify(repo => repo.ObterIdAsync(cliente.Id), Times.Once);
        _mockRepo.Verify(repo => repo.ObterEmailAsync(It.IsAny<string>()), Times.Never);
        _mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoNenhumIdentificadorForFornecido()
    {
        // Arrange
        SetupValidatorSuccess();
        var cliente = new Cliente
        {
            Id = Guid.Empty,
            Email = null,
            CpfCnpj = null
            // Todos os identificadores vazios/nulos
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AtualizarAsync(cliente));
        Assert.Contains("não existe", ex.Message.ToLower());
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
    }
}
