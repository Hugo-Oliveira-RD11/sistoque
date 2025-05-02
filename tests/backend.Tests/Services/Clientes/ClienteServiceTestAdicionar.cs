
using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestAdicionar
{
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
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "Hugo",
      CpfCnpj = cpfCnpj,
      Telefone = "61123456789",
      Email = "hugo@boss.com",
      SenhaHash = "12345",
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
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
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = nome,
      CpfCnpj = "12345678912",
      Telefone = "61123456789",
      Email = "hugo@boss.com",
      SenhaHash = "12345",
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
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
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "hugo",
      CpfCnpj = "12345678912",
      Telefone = telefone,
      Email = "hugo@boss.com",
      SenhaHash = "12345",
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }

  [Theory]
  [InlineData("123456789123")]
  [InlineData("12345678911")]
  [InlineData("-1")]
  public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoTelefoneTiverMenosOuMaisNumeros(string telefone)
  {
    // arrange
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "hugo",
      CpfCnpj = "12345678912",
      Telefone = telefone,
      Email = "hugo@boss.com",
      SenhaHash = "12345",
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
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
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "hugo",
      CpfCnpj = "12345678912",
      Telefone = telefone,
      Email = "hugo@boss.com",
      SenhaHash = "12345",
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoNomeForNuloOuVazio(string? nome)
  {
    // arrange
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = nome,
      CpfCnpj = "12345678912",
      Telefone = "12345678912",
      Email = "hugo@boss.com",
      SenhaHash = "12345",
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoEmailForNuloOuVazio(string? email)
  {
    // arrange
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "Hugo",
      CpfCnpj = "12345678912",
      Telefone = "12345678912",
      Email = email,
      SenhaHash = "12345",
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoSenhaForNuloOuVazio(string? senha)
  {
    // arrange
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "Hugo",
      CpfCnpj = "12345678912",
      Telefone = "12345678912",
      Email = "hugo@boss.com",
      SenhaHash = senha,
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
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
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "Hugo",
      CpfCnpj = "12345678912",
      Telefone = "12345678912",
      Email = "hugo@boss.com",
      SenhaHash = senha,
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }

  [Theory]
  [InlineData("a2345")]
  [InlineData(".a345")]
  [InlineData(". 345")]
  [InlineData(".@345")]
  public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoSenhaForMenorQueSeisCaracteres(string senha)
  {
    // arrange
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "Hugo",
      CpfCnpj = "12345678912",
      Telefone = "12345678912",
      Email = "hugo@boss.com",
      SenhaHash = senha,
      Role = "admin"
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }

  [Theory]
  [InlineData("")]
  [InlineData(null)]
  public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoRoleForNulaOuVazia(string? role)
  {
    // arrange
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "Hugo",
      CpfCnpj = "12345678912",
      Telefone = "12345678912",
      Email = "hugo@boss.com",
      SenhaHash = "12345678",
      Role = role
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }

  [Theory]
  [InlineData("admin")]
  [InlineData("user")]
  public async Task AdicionarClienteAsync_DeveAceitar_QuandoRoleForAdminOuUser(string role)
  {
    // arrange
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "Hugo",
      CpfCnpj = "12345678912",
      Telefone = "12345678912",
      Email = "hugo@boss.com",
      SenhaHash = "12345678",
      Role = role
    };

    await service.AdicionarAsync(clienteInvalido);

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Once);
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
    var mockRepo = new Mock<IClienteRepository>();
    var service = new ClienteService(mockRepo.Object);
    var clienteInvalido = new Cliente
    {
      Id = Guid.NewGuid(),
      Nome = "Hugo",
      CpfCnpj = "12345678912",
      Telefone = "12345678912",
      Email = "hugo@boss.com",
      SenhaHash = "12345678",
      Role = role
    };

    await Assert.ThrowsAsync<ArgumentException>(() => service.AdicionarAsync(clienteInvalido));

    //assrt
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }

  [Theory]
  [InlineData("hugo@boss.com")]
  public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoEmailJaEsteSidoUsado(string email)
  {
    var mockRepo = new Mock<IClienteRepository>();
    Guid id = Guid.NewGuid();
    var clienteInserido = new Cliente
    {
      Id = id,
      Nome = "Hugo",
      CpfCnpj = "12345678912",
      Telefone = "12345678912",
      Email = email,
      SenhaHash = "123456789",
      Role = "admin"
    };
    var novoCliente = new Cliente
    {
      Id = id,
      Nome = "Hugo",
      CpfCnpj = "12345678913",
      Telefone = "98765432198",
      Email = email,
      SenhaHash = "987654321",
      Role = "admin"
    };
    mockRepo.Setup(repo => repo.ObterEmailAsync(novoCliente.Email)).ReturnsAsync(clienteInserido);
    var service = new ClienteService(mockRepo.Object);

    var excecao = await Assert.ThrowsAsync<ArgumentException>( () => service.AdicionarAsync(novoCliente));

    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }

  [Theory]
  [InlineData("12345678912")] // testa cpf
  [InlineData("12345678912345")] // testa cnpj
  public async Task AdicionarClienteAsync_DeveLancaExcecao_QuandoCpfCnpjJaEsteSidoUsado(string cpfCnpj)
  {
    var mockRepo = new Mock<IClienteRepository>();
    Guid id = Guid.NewGuid();
    var clienteInserido = new Cliente
    {
      Id = id,
      Nome = "Hugo",
      CpfCnpj = cpfCnpj,
      Telefone = "12345678912",
      Email = "hugo@boss1.com",
      SenhaHash = "123456789",
      Role = "admin"
    };
    var novoCliente = new Cliente
    {
      Id = id,
      Nome = "Hugo",
      CpfCnpj = cpfCnpj,
      Telefone = "98765432198",
      Email = "hugo@boss.com",
      SenhaHash = "987654321",
      Role = "admin"
    };
    mockRepo.Setup(repo => repo.ObterEmailAsync(novoCliente.Email)).ReturnsAsync(null as Cliente);
    mockRepo.Setup(repo => repo.ObterCpfCnpjAsync(clienteInserido.CpfCnpj)).ReturnsAsync(clienteInserido);
    var service = new ClienteService(mockRepo.Object);

    var excecao = await Assert.ThrowsAsync<ArgumentException>( () => service.AdicionarAsync(novoCliente));

    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
  }
}
