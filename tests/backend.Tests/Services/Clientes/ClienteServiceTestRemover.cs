using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using FluentValidation;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestRemover
{
    private ClienteService CriarServiceComMocks(
        Mock<IClienteRepository> mockRepo,
        Mock<IValidator<Cliente>>? mockValidator = null)
    {
        mockValidator ??= new Mock<IValidator<Cliente>>();
        // Como o validador não é usado no método Remover, qualquer comportamento serve.
        mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Cliente>(), default))
                     .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        return new ClienteService(mockRepo.Object, mockValidator.Object);
    }

    [Fact]
    public async Task RemoverIdAsync_DeveFuncionar_QuandoIdForValido()
    {
        var mockRepo = new Mock<IClienteRepository>();
        var idValido = Guid.NewGuid();
        var clienteMock = new Cliente
        {
            Id = idValido,
            Nome = "Hugo",
            CpfCnpj = "12345678912",
            Telefone = "12345678912",
            Email = "hugo@boss1.com",
            SenhaHash = "123456789",
            Role = "admin"
        };

        mockRepo.Setup(repo => repo.ObterIdAsync(idValido))
                .ReturnsAsync(clienteMock);
        mockRepo.Setup(repo => repo.RemoverAsync(idValido))
                .Returns(Task.CompletedTask);

        var service = CriarServiceComMocks(mockRepo);

        var exception = await Record.ExceptionAsync(() => service.RemoverIdAsync(idValido));

        Assert.Null(exception);
        mockRepo.Verify(repo => repo.ObterIdAsync(idValido), Times.Once);
        mockRepo.Verify(repo => repo.RemoverAsync(idValido), Times.Once);
    }

    [Fact]
    public async Task RemoverIdAsync_DeveLancarExcecao_QuandoIdNaoExiste()
    {
        var mockRepo = new Mock<IClienteRepository>();
        var idInvalido = Guid.NewGuid();

        mockRepo.Setup(repo => repo.ObterIdAsync(idInvalido))
                .ReturnsAsync((Cliente?)null);

        var service = CriarServiceComMocks(mockRepo);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RemoverIdAsync(idInvalido));

        mockRepo.Verify(repo => repo.ObterIdAsync(idInvalido), Times.Once);
        mockRepo.Verify(repo => repo.RemoverAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task RemoverIdAsync_DeveLancarExcecao_QuandoRoleNaoForAdmin()
    {
        var mockRepo = new Mock<IClienteRepository>();
        var idValido = Guid.NewGuid();

        var clienteMock = new Cliente
        {
            Id = idValido,
            Nome = "Hugo",
            CpfCnpj = "12345678912",
            Telefone = "12345678912",
            Email = "hugo@boss1.com",
            SenhaHash = "123456789",
            Role = "user"
        };

        mockRepo.Setup(repo => repo.ObterIdAsync(idValido))
                .ReturnsAsync(clienteMock);

        var service = CriarServiceComMocks(mockRepo);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RemoverIdAsync(idValido));

        mockRepo.Verify(repo => repo.ObterIdAsync(idValido), Times.Once);
        mockRepo.Verify(repo => repo.RemoverAsync(It.IsAny<Guid>()), Times.Never);
    }
}
