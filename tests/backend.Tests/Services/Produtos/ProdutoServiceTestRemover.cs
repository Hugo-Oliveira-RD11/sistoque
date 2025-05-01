using backend.Models;
using backend.Repository;
using backend.Services.Produtos;
using Moq;

namespace backend.Tests.Services.Produtos;

public class ProdutoServiceTestRemover
{
  [Fact]
  public async Task RemoverProdutoAsync_DeveRemover_QuandoExistirUm()
  {
    var mockRepo = new Mock<IProdutoRepository>();
    var id = Guid.NewGuid();
    var produtoRemover = new Produto
    {
        Id = id,
        Nome = "Produto para deletar",
        Preco = 10,
        QuantidadeDisponivel = 5,
    };

    mockRepo.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(produtoRemover);

    var service = new ProdutoService(mockRepo.Object);

    await service.RemoverProdutoAsync(id);

    mockRepo.Verify(repo => repo.ObterPorIdAsync(id), Times.Once);
    mockRepo.Verify(repo => repo.RemoverAsync(id), Times.Once);
  }

  [Theory]
  [InlineData("08bb54f0-2a6d-4a88-ba16-1c0bc9fb5766")] // id inexistente
  [InlineData("1a9f04a8-2d1a-485a-b2fd-3fec29aafbff")]
  public async Task RemoverProdutoAsync_DeveLancaExcecao_QuandoNaoExistirOuIdErrado(string stringId)
  {
    var mockRepo = new Mock<IProdutoRepository>();
    var Id = Guid.Parse(stringId);
    mockRepo.Setup(repo => repo.ObterPorIdAsync(Id)).ReturnsAsync((Produto?)null);

    var service = new ProdutoService(mockRepo.Object);

    var act = async () => await service.RemoverProdutoAsync(Id);

    var excecao = await Assert.ThrowsAsync<ArgumentException>(act);
    mockRepo.Verify(repo => repo.ObterPorIdAsync(Id), Times.Once);
    mockRepo.Verify(repo => repo.RemoverAsync(Id), Times.Never);
  }

}
