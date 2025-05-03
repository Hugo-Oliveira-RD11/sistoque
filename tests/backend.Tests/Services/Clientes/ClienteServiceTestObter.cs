using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using FluentValidation;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestObter
{
    private ClienteService CriarServiceComMocks(
        Mock<IClienteRepository> mockRepo,
        Mock<IValidator<Cliente>>? mockValidator = null)
    {
        mockValidator ??= new Mock<IValidator<Cliente>>();
        mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Cliente>(), default))
                     .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        return new ClienteService(mockRepo.Object, mockValidator.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarCliente_QuandoIdForValido()
    {
        var mockRepo = new Mock<IClienteRepository>();
        var id = Guid.NewGuid();
        var clienteEsperado = new Cliente
        {
            Id = id,
            Nome = "Hugo",
            CpfCnpj = "12345678912",
            Telefone = "98765432198",
            Email = "hugo@boss.com",
            SenhaHash = "987654321",
            Role = "admin"
        };

        mockRepo.Setup(repo => repo.ObterIdAsync(id)).ReturnsAsync(clienteEsperado);
        var service = CriarServiceComMocks(mockRepo);

        var resultado = await service.ObterPorIdAsync(clienteEsperado.Id);

        Assert.Equivalent(clienteEsperado, resultado);
    }

    [Fact]
    public async Task ObterPorEmailAsync_DeveRetornarCliente_QuandoEmailForValido()
    {
        var mockRepo = new Mock<IClienteRepository>();
        var clienteEsperado = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Hugo",
            CpfCnpj = "12345678912",
            Telefone = "98765432198",
            Email = "hugo@boss.com",
            SenhaHash = "987654321",
            Role = "admin"
        };

        mockRepo.Setup(repo => repo.ObterEmailAsync(clienteEsperado.Email)).ReturnsAsync(clienteEsperado);
        var service = CriarServiceComMocks(mockRepo);

        var resultado = await service.ObterPorEmailAsync(clienteEsperado.Email);

        Assert.Equivalent(clienteEsperado, resultado);
    }

    [Theory]
    [InlineData("12345678912")]
    [InlineData("12345678912345")]
    public async Task ObterPorCpfCnpjAsync_DeveRetornarCliente_QuandoCpfOuCnpjForValido(string cpfCnpj)
    {
        var mockRepo = new Mock<IClienteRepository>();
        var clienteEsperado = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = "Hugo",
            CpfCnpj = cpfCnpj,
            Telefone = "98765432198",
            Email = "hugo@boss.com",
            SenhaHash = "987654321",
            Role = "admin"
        };

        mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(clienteEsperado.CpfCnpj)).ReturnsAsync(clienteEsperado);
        var service = CriarServiceComMocks(mockRepo);

        var resultado = await service.ObterPorCpfCnpjAsync(clienteEsperado.CpfCnpj);

        Assert.Equivalent(clienteEsperado, resultado);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoIdForInvalido()
    {
        var mockRepo = new Mock<IClienteRepository>();
        var id = Guid.NewGuid();

        mockRepo.Setup(repo => repo.ObterIdAsync(id)).ReturnsAsync((Cliente?)null);
        var service = CriarServiceComMocks(mockRepo);

        var resultado = await service.ObterPorIdAsync(id);

        Assert.Null(resultado);
    }

    [Theory]
    [InlineData("hugo@boss.com")]
    public async Task ObterPorEmailAsync_DeveRetornarNull_QuandoEmailForInvalido(string emailInvalido)
    {
        var mockRepo = new Mock<IClienteRepository>();

        mockRepo.Setup(repo => repo.ObterEmailAsync(emailInvalido)).ReturnsAsync((Cliente?)null);
        var service = CriarServiceComMocks(mockRepo);

        var resultado = await service.ObterPorEmailAsync(emailInvalido);

        Assert.Null(resultado);
    }

    [Theory]
    [InlineData("12345678912")]
    [InlineData("12345678912345")]
    public async Task ObterPorCpfCnpjAsync_DeveRetornarNull_QuandoCpfOuCnpjForInvalido(string cpfCnpjInvalido)
    {
        var mockRepo = new Mock<IClienteRepository>();

        mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(cpfCnpjInvalido)).ReturnsAsync((Cliente?)null);
        var service = CriarServiceComMocks(mockRepo);

        var resultado = await service.ObterPorCpfCnpjAsync(cpfCnpjInvalido);

        Assert.Null(resultado);
    }
}
