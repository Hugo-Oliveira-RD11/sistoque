using backend.Models;
using backend.Repository;
using backend.Services.Clientes;
using Moq;

namespace backend.Tests.Services.Clientes;

public class ClienteServiceTestAtualizar
{
  [Fact]
  public async Task AtualizarAsync_DeveLancaExcecao_QuandoClienteTentarAtualizarForIgualJaInserido()
  {
    var mockRepo = new Mock<IClienteRepository>();
    var id = Guid.NewGuid();
    var clienteInserido = new Cliente
    {
      Id = id,
      Nome = "Hugo",
      CpfCnpj = "12345678913",
      Telefone = "98765432198",
      Email = "hugo@boss.com",
      SenhaHash = "987654321",
      Role = "admin"
    };

    mockRepo.Setup(repo => repo.ObterIdAsync(clienteInserido.Id)).ReturnsAsync(clienteInserido);
    mockRepo.Setup(repo => repo.ObterEmailAsync(clienteInserido.Email)).ReturnsAsync(clienteInserido);
    var service = new ClienteService(mockRepo.Object);

    var excecao = await Assert.ThrowsAsync<ArgumentException>( () => service.AtualizarAsync(clienteInserido));

    mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
  }
}
