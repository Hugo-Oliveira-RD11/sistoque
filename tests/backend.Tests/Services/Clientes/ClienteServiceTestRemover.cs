
using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestRemover
{
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
                .Returns(Task.CompletedTask); // Para métodos async que retornam Task

        var service = new ClienteService(mockRepo.Object);

        var exception = await Record.ExceptionAsync(() => service.RemoverIdAsync(idValido));

        Assert.Null(exception);
        mockRepo.Verify(repo => repo.ObterIdAsync(idValido), Times.Once);
        mockRepo.Verify(repo => repo.RemoverAsync(idValido), Times.Once);
    }


    [Fact]
    public async Task RemoverIdAsync_DeveLancaExcecao_QuandoIdForInvalido()
    {
        var mockRepo = new Mock<IClienteRepository>();
        var idInvalido = Guid.NewGuid();

        mockRepo.Setup(repo => repo.RemoverAsync(idInvalido))
                .Returns(Task.CompletedTask);
        var service = new ClienteService(mockRepo.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RemoverIdAsync(idInvalido));

        mockRepo.Verify(repo => repo.ObterIdAsync(idInvalido), Times.Once);
        mockRepo.Verify(repo => repo.RemoverAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task RemoverIdAsync_DeveFuncionar_QuandoRoleForAdmin()
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
                .Returns(Task.CompletedTask); // Para métodos async que retornam Task

        var service = new ClienteService(mockRepo.Object);

        var exception = await Record.ExceptionAsync(() => service.RemoverIdAsync(idValido));

        Assert.Null(exception);
        mockRepo.Verify(repo => repo.ObterIdAsync(idValido), Times.Once);
        mockRepo.Verify(repo => repo.RemoverAsync(idValido), Times.Once);
    }

    [Theory]
    [InlineData("user")]
    public async Task RemoverIdAsync_DeveLancaExcecao_QuandoRoleForDiferenteDeAdmin(string role)
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
            Role = role
        };
        mockRepo.Setup(repo => repo.ObterIdAsync(idValido))
            .ReturnsAsync(clienteMock);

        mockRepo.Setup(repo => repo.RemoverAsync(idValido))
                .Returns(Task.CompletedTask); // Para métodos async que retornam Task

        var service = new ClienteService(mockRepo.Object);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RemoverIdAsync(idValido)
        );

        mockRepo.Verify(repo => repo.ObterIdAsync(idValido), Times.Once);
        mockRepo.Verify(repo => repo.RemoverAsync(It.IsAny<Guid>()), Times.Never);
    }

}
