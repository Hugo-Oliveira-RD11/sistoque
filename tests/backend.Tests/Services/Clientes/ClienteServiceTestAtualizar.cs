using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using backend.Validacao;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestAtualizar
{
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

    private Mock<IValidator<Cliente>> CriarValidadorMock(bool valido = true)
    {
        var validadorMock = new Mock<IValidator<Cliente>>();
        var resultado = new ValidationResult();

        if (!valido)
        {
            resultado.Errors.Add(new ValidationFailure("propriedade", "mensagem de erro"));
        }

        validadorMock.Setup(v => v.ValidateAsync(It.IsAny<Cliente>(), default))
                    .ReturnsAsync(resultado);

        return validadorMock;
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoValidacaoFalhar()
    {
        // Arrange
        var mockRepo = new Mock<IClienteRepository>();
        var validadorMock = CriarValidadorMock(valido: false);
        var service = new ClienteService(mockRepo.Object, validadorMock.Object);
        var cliente = CriarClienteValido();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AtualizarAsync(cliente));
        mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoClienteNaoExistir()
    {
        // Arrange
        var mockRepo = new Mock<IClienteRepository>();
        var validadorMock = CriarValidadorMock();
        var service = new ClienteService(mockRepo.Object, validadorMock.Object);
        var cliente = CriarClienteValido();

        mockRepo.Setup(repo => repo.ObterIdAsync(cliente.Id)).ReturnsAsync((Cliente?)null);
        mockRepo.Setup(repo => repo.ObterEmailAsync(cliente.Email)).ReturnsAsync((Cliente?)null);
        mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(cliente.CpfCnpj)).ReturnsAsync((Cliente?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.AtualizarAsync(cliente));
        Assert.Contains("não existe", ex.Message.ToLower());
        mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoNenhumaMudanca()
    {
        // Arrange
        var mockRepo = new Mock<IClienteRepository>();
        var validadorMock = CriarValidadorMock();
        var service = new ClienteService(mockRepo.Object, validadorMock.Object);
        var clienteExistente = CriarClienteValido();
        var clienteAtualizacao = CriarClienteValido(); // Cópia exata

        mockRepo.Setup(repo => repo.ObterIdAsync(clienteExistente.Id))
               .ReturnsAsync(clienteExistente);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.AtualizarAsync(clienteAtualizacao));
        mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizar_QuandoClienteExistirEComMudancas()
    {
        // Arrange
        var mockRepo = new Mock<IClienteRepository>();
        var validadorMock = CriarValidadorMock();
        var service = new ClienteService(mockRepo.Object, validadorMock.Object);
        var clienteOriginal = CriarClienteValido();
        var clienteModificado = CriarClienteComDiferencas(clienteOriginal);

        mockRepo.Setup(repo => repo.ObterIdAsync(clienteOriginal.Id))
               .ReturnsAsync(clienteOriginal);

        // Act
        await service.AtualizarAsync(clienteModificado);

        // Assert
        mockRepo.Verify(repo => repo.AtualizarAsync(clienteModificado), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveBuscarPorId_QuandoIdForValido()
    {
        // Arrange
        var mockRepo = new Mock<IClienteRepository>();
        var validadorMock = CriarValidadorMock();
        var service = new ClienteService(mockRepo.Object, validadorMock.Object);
        var cliente = CriarClienteValido();
        var clienteModificado = CriarClienteComDiferencas(cliente);

        mockRepo.Setup(repo => repo.ObterIdAsync(cliente.Id))
               .ReturnsAsync(cliente);

        // Act
        await service.AtualizarAsync(clienteModificado);

        // Assert
        mockRepo.Verify(repo => repo.ObterIdAsync(cliente.Id), Times.Once);
        mockRepo.Verify(repo => repo.ObterEmailAsync(It.IsAny<string>()), Times.Never);
        mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveBuscarPorEmail_QuandoIdForVazio()
    {
        // Arrange
        var mockRepo = new Mock<IClienteRepository>();
        var validadorMock = CriarValidadorMock();
        var service = new ClienteService(mockRepo.Object, validadorMock.Object);
        var cliente = CriarClientePadrao(); // Id vazio
        var clienteModificado = CriarClienteComDiferencas(cliente);

        mockRepo.Setup(repo => repo.ObterEmailAsync(cliente.Email))
               .ReturnsAsync(cliente);

        // Act
        await service.AtualizarAsync(clienteModificado);

        // Assert
        mockRepo.Verify(repo => repo.ObterEmailAsync(cliente.Email), Times.Once);
        mockRepo.Verify(repo => repo.ObterIdAsync(It.IsAny<Guid>()), Times.Never);
        mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveBuscarPorCpfCnpj_QuandoIdForVazioEEmailForVazio()
    {
        // Arrange
        var mockRepo = new Mock<IClienteRepository>();
        var validadorMock = CriarValidadorMock();
        var service = new ClienteService(mockRepo.Object, validadorMock.Object);
        var cliente = CriarClientePadrao(); // Id vazio
        cliente.Email = null; // Email vazio
        var clienteModificado = CriarClienteComDiferencas(cliente);

        mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(cliente.CpfCnpj))
               .ReturnsAsync(cliente);

        // Act
        await service.AtualizarAsync(clienteModificado);

        // Assert
        mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(cliente.CpfCnpj), Times.Once);
        mockRepo.Verify(repo => repo.ObterIdAsync(It.IsAny<Guid>()), Times.Never);
        mockRepo.Verify(repo => repo.ObterEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveSeguirOrdemCorretaDeBusca_IdEmailCpfCnpj()
    {
        // Arrange
        var mockRepo = new Mock<IClienteRepository>();
        var validadorMock = CriarValidadorMock();
        var service = new ClienteService(mockRepo.Object, validadorMock.Object);
        var cliente = CriarClienteValido(); // Com ID válido
        var clienteModificado = CriarClienteComDiferencas(cliente);

        mockRepo.Setup(repo => repo.ObterIdAsync(cliente.Id))
               .ReturnsAsync(cliente);

        // Act
        await service.AtualizarAsync(clienteModificado);

        // Assert - Verifica que só chamou ObterPorIdAsync
        mockRepo.Verify(repo => repo.ObterIdAsync(cliente.Id), Times.Once);
        mockRepo.Verify(repo => repo.ObterEmailAsync(It.IsAny<string>()), Times.Never);
        mockRepo.Verify(repo => repo.ObterCpfCnpjAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoNenhumIdentificadorForFornecido()
    {
        // Arrange
        var mockRepo = new Mock<IClienteRepository>();
        var validadorMock = CriarValidadorMock();
        var service = new ClienteService(mockRepo.Object, validadorMock.Object);
        var cliente = new Cliente
        {
            Id = Guid.Empty,
            Email = null,
            CpfCnpj = null
            // Todos os identificadores vazios/nulos
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.AtualizarAsync(cliente));
        Assert.Contains("não existe", ex.Message.ToLower());
        mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
    }
}
