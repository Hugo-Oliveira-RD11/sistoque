using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using backend.Services.Clientes.Auth;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestAdicionar
{
    private readonly Mock<IClienteRepository> _mockRepo;
    private readonly Mock<ITokenService> _mockToken;
    private readonly Mock<IHashServices> _mockHash;
    private readonly Mock<IValidator<Cliente>> _mockValidator;
    private readonly ClienteService _service;

    public ClienteServiceTestAdicionar()
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

    [Theory]
    [InlineData("12345678")]
    [InlineData("1")]
    [InlineData("123456789123456")]
    [InlineData("123.456.789-12")]
    [InlineData("123.456.789/12")]
    [InlineData("123456789/12")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoCpfOuCnpjInvalido(string cpfCnpj)
    {
        // arrange
        SetupValidatorFailure("CPF/CNPJ inválido");
        var clienteInvalido = new Cliente { CpfCnpj = cpfCnpj };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("1* dom pedro")]
    [InlineData("1 dom pedro")]
    [InlineData("Dom-pedro")]
    [InlineData("-Dom pedro")]
    [InlineData(".-Dom pedro")]
    [InlineData("Am@nd@")]
    [InlineData("Dom pedro/1.v")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoNomeTiverCaracteresEspeciaisExcecaoPonto(string nome)
    {
        // arrange
        SetupValidatorFailure("Nome contém caracteres inválidos");
        var clienteInvalido = new Cliente { Nome = nome };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("123456789(6")]
    [InlineData(".1234567891")]
    [InlineData("@1234567891")]
    [InlineData("#1234567891")]
    [InlineData("$1234567891")]
    [InlineData("%1234567891")]
    [InlineData("123456789-1")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoTelefoneTiverCaracteresEspeciais(string telefone)
    {
        // arrange
        SetupValidatorFailure("Telefone contém caracteres inválidos");
        var clienteInvalido = new Cliente { Telefone = telefone };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("123456789123")]
    [InlineData("12345678911")]
    [InlineData("-1")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoTelefoneTiverMenosOuMaisNumeros(string telefone)
    {
        // arrange
        SetupValidatorFailure("Telefone deve ter 11 dígitos");
        var clienteInvalido = new Cliente { Telefone = telefone };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("a12345678912")]
    [InlineData("12345678912a")]
    [InlineData("12345678912A")]
    [InlineData("123456a78912")]
    [InlineData("123456A78912")]
    [InlineData("A12345678912")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoTelefoneTiverLetras(string telefone)
    {
        // arrange
        SetupValidatorFailure("Telefone deve conter apenas números");
        var clienteInvalido = new Cliente { Telefone = telefone };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoNomeForNuloOuVazio(string? nome)
    {
        // arrange
        SetupValidatorFailure("Nome é obrigatório");
        var clienteInvalido = new Cliente { Nome = nome };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoEmailForNuloOuVazio(string? email)
    {
        // arrange
        SetupValidatorFailure("Email é obrigatório");
        var clienteInvalido = new Cliente { Email = email };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoSenhaForNuloOuVazio(string? senha)
    {
        // arrange
        SetupValidatorFailure("Senha é obrigatória");
        var clienteInvalido = new Cliente { SenhaHash = senha };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("1aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData(".aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa ")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoSenhaForMaiorQueSessentaCaracteres(string senha)
    {
        // arrange
        SetupValidatorFailure("Senha deve ter no máximo 60 caracteres");
        var clienteInvalido = new Cliente { SenhaHash = senha };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("a2345")]
    [InlineData(".a345")]
    [InlineData(". 345")]
    [InlineData(".@345")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoSenhaForMenorQueSeisCaracteres(string senha)
    {
        // arrange
        SetupValidatorFailure("Senha deve ter no mínimo 6 caracteres");
        var clienteInvalido = new Cliente { SenhaHash = senha };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoRoleForNulaOuVazia(string? role)
    {
        // arrange
        SetupValidatorFailure("Role é obrigatória");
        var clienteInvalido = new Cliente { Role = role };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("user")]
    public async Task AdicionarClienteAsync_DeveAceitar_QuandoRoleForAdminOuUser(string role)
    {
        // arrange
        SetupValidatorSuccess();
        var clienteValido = new Cliente { Role = role };

        // act
        await _service.AdicionarAsync(clienteValido);

        // assert
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Theory]
    [InlineData(" admin")]
    [InlineData(" user")]
    [InlineData("User")]
    [InlineData("Admin")]
    [InlineData("User.")]
    [InlineData("Admin.")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoRoleForDiferenteDeAdminOuUser(string role)
    {
        // arrange
        SetupValidatorFailure("Role deve ser 'admin' ou 'user'");
        var clienteInvalido = new Cliente { Role = role };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteInvalido));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("hugo@boss.com")]
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoEmailJaEsteSidoUsado(string email)
    {
        // arrange
        SetupValidatorSuccess();
        var clienteExistente = new Cliente { Email = email };
        _mockRepo.Setup(repo => repo.ObterEmailAsync(email))
            .ReturnsAsync(clienteExistente);

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteExistente));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Theory]
    [InlineData("12345678912")] // testa cpf
    [InlineData("12345678912345")] // testa cnpj
    public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoCpfCnpjJaEsteSidoUsado(string cpfCnpj)
    {
        // arrange
        SetupValidatorSuccess();
        var clienteExistente = new Cliente { CpfCnpj = cpfCnpj };
        _mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(cpfCnpj))
            .ReturnsAsync(clienteExistente);

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AdicionarAsync(clienteExistente));
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
    }
}
