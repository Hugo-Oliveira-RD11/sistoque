using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using backend.Services.Clientes.Auth;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestRemover
{
    private readonly Mock<IClienteRepository> _mockRepo;
    private readonly Mock<ITokenService> _mockToken;
    private readonly Mock<IHashServices> _mockHash;
    private readonly Mock<IValidator<Cliente>> _mockValidator;
    private readonly ClienteService _service;

    public ClienteServiceTestRemover()
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

    private Cliente CriarClientePadrao(Guid? id = null, string role = "admin")
    {
        return new Cliente
        {
            Id = id ?? Guid.NewGuid(),
            Nome = "Cliente Teste",
            Email = "teste@teste.com",
            CpfCnpj = "12345678901",
            Telefone = "11999999999",
            SenhaHash = "123456",
            Role = role
        };
    }

    [Fact]
    public async Task RemoverIdAsync_DeveFuncionar_QuandoIdForValido()
    {
        // Arrange
        SetupValidatorSuccess();
        var idValido = Guid.NewGuid();
        var clienteMock = CriarClientePadrao(idValido);

        _mockRepo.Setup(repo => repo.ObterIdAsync(idValido))
                 .ReturnsAsync(clienteMock);
        _mockRepo.Setup(repo => repo.RemoverAsync(idValido))
                 .Returns(Task.CompletedTask);

        // Act
        var exception = await Record.ExceptionAsync(() => _service.RemoverIdAsync(idValido));

        // Assert
        Assert.Null(exception);
        _mockRepo.Verify(repo => repo.ObterIdAsync(idValido), Times.Once);
        _mockRepo.Verify(repo => repo.RemoverAsync(idValido), Times.Once);
    }

    [Fact]
    public async Task RemoverIdAsync_DeveLancarExcecao_QuandoIdNaoExiste()
    {
        // Arrange
        SetupValidatorSuccess();
        var idInvalido = Guid.NewGuid();

        _mockRepo.Setup(repo => repo.ObterIdAsync(idInvalido))
                 .ReturnsAsync((Cliente?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverIdAsync(idInvalido));

        _mockRepo.Verify(repo => repo.ObterIdAsync(idInvalido), Times.Once);
        _mockRepo.Verify(repo => repo.RemoverAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task RemoverIdAsync_DeveLancarExcecao_QuandoRoleNaoForAdmin()
    {
        // Arrange
        SetupValidatorSuccess();
        var idValido = Guid.NewGuid();
        var clienteMock = CriarClientePadrao(idValido, role: "user");

        _mockRepo.Setup(repo => repo.ObterIdAsync(idValido))
                 .ReturnsAsync(clienteMock);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverIdAsync(idValido));

        _mockRepo.Verify(repo => repo.ObterIdAsync(idValido), Times.Once);
        _mockRepo.Verify(repo => repo.RemoverAsync(It.IsAny<Guid>()), Times.Never);
    }
}
