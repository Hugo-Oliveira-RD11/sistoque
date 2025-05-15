using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using backend.Services.Clientes.Auth;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestObter
{
    private readonly Mock<IClienteRepository> _mockRepo;
    private readonly Mock<ITokenService> _mockToken;
    private readonly Mock<IHashServices> _mockHash;
    private readonly Mock<IValidator<Cliente>> _mockValidator;
    private readonly ClienteService _service;

    public ClienteServiceTestObter()
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

    private Cliente CriarClientePadrao(Guid? id = null)
    {
        return new Cliente
        {
            Id = id ?? Guid.NewGuid(),
            Nome = "Cliente Teste",
            Email = "teste@teste.com",
            CpfCnpj = "12345678901",
            Telefone = "11999999999",
            SenhaHash = "123456",
            Role = "user"
        };
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarCliente_QuandoIdForValido()
    {
        // Arrange
        SetupValidatorSuccess();
        var clienteEsperado = CriarClientePadrao();

        _mockRepo.Setup(repo => repo.ObterIdAsync(clienteEsperado.Id))
               .ReturnsAsync(clienteEsperado);

        // Act
        var resultado = await _service.ObterPorIdAsync(clienteEsperado.Id);

        // Assert
        Assert.Equivalent(clienteEsperado, resultado);
        _mockRepo.Verify(repo => repo.ObterIdAsync(clienteEsperado.Id), Times.Once);
    }

    [Fact]
    public async Task ObterPorEmailAsync_DeveRetornarCliente_QuandoEmailForValido()
    {
        // Arrange
        SetupValidatorSuccess();
        var clienteEsperado = CriarClientePadrao();

        _mockRepo.Setup(repo => repo.ObterEmailAsync(clienteEsperado.Email))
               .ReturnsAsync(clienteEsperado);

        // Act
        var resultado = await _service.ObterPorEmailAsync(clienteEsperado.Email);

        // Assert
        Assert.Equivalent(clienteEsperado, resultado);
        _mockRepo.Verify(repo => repo.ObterEmailAsync(clienteEsperado.Email), Times.Once);
    }

    [Theory]
    [InlineData("12345678912")] // CPF
    [InlineData("12345678912345")] // CNPJ
    public async Task ObterPorCpfCnpjAsync_DeveRetornarCliente_QuandoCpfOuCnpjForValido(string cpfCnpj)
    {
        // Arrange
        SetupValidatorSuccess();
        var clienteEsperado = CriarClientePadrao();
        clienteEsperado.CpfCnpj = cpfCnpj;

        _mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(cpfCnpj))
               .ReturnsAsync(clienteEsperado);

        // Act
        var resultado = await _service.ObterPorCpfCnpjAsync(cpfCnpj);

        // Assert
        Assert.Equivalent(clienteEsperado, resultado);
        _mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(cpfCnpj), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoIdForInvalido()
    {
        // Arrange
        SetupValidatorSuccess();
        var idInvalido = Guid.NewGuid();

        _mockRepo.Setup(repo => repo.ObterIdAsync(idInvalido))
               .ReturnsAsync((Cliente?)null);

        // Act
        var resultado = await _service.ObterPorIdAsync(idInvalido);

        // Assert
        Assert.Null(resultado);
        _mockRepo.Verify(repo => repo.ObterIdAsync(idInvalido), Times.Once);
    }

    [Fact]
    public async Task ObterPorEmailAsync_DeveRetornarNull_QuandoEmailForInvalido()
    {
        // Arrange
        SetupValidatorSuccess();
        var emailInvalido = "email@invalido.com";

        _mockRepo.Setup(repo => repo.ObterEmailAsync(emailInvalido))
               .ReturnsAsync((Cliente?)null);

        // Act
        var resultado = await _service.ObterPorEmailAsync(emailInvalido);

        // Assert
        Assert.Null(resultado);
        _mockRepo.Verify(repo => repo.ObterEmailAsync(emailInvalido), Times.Once);
    }

    [Theory]
    [InlineData("12345678912")] // CPF inválido
    [InlineData("12345678912345")] // CNPJ inválido
    public async Task ObterPorCpfCnpjAsync_DeveRetornarNull_QuandoCpfOuCnpjForInvalido(string cpfCnpjInvalido)
    {
        // Arrange
        SetupValidatorSuccess();

        _mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(cpfCnpjInvalido))
               .ReturnsAsync((Cliente?)null);

        // Act
        var resultado = await _service.ObterPorCpfCnpjAsync(cpfCnpjInvalido);

        // Assert
        Assert.Null(resultado);
        _mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(cpfCnpjInvalido), Times.Once);
    }
}
