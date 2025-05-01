using backend.Models;
using backend.Repository;
using backend.Services.Produtos;
using Moq;

namespace backend.Tests.Services.Produtos;

public class ProdutoServiceTest
{

  [Fact]
  public async Task AdicionarProdutoAsync_DeveAdicionarProduto_QuandoForValido()
  {
    // Arrange
    var mockRepo = new Mock<IProdutoRepository>();
    var service = new ProdutoService(mockRepo.Object);

    var produtoValido = new Produto
    {
      Id = Guid.NewGuid(),
      Nome = "Produto Teste",
      Preco = 10,
      QuantidadeDisponivel = 5,
    };

    // Act
    await service.AdicionarProdutoAsync(produtoValido);

    // assert
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Once);
  }

  [Theory]
  [InlineData(0,10)]
  [InlineData(0,10.99)]
  [InlineData(10,0.0)]
  [InlineData(0,0.0)]
  public async Task AdicionarProdutoAsync_DeveAdicionarProduto_QuandoPrecoOuQuantidadeForZero(int quantidade, decimal preco)
  {
    // Arrange
    var mockRepo = new Mock<IProdutoRepository>();
    var service = new ProdutoService(mockRepo.Object);

    var produtoValido = new Produto
    {
      Id = Guid.NewGuid(),
      Nome = "Produto Teste",
      Preco = preco,
      QuantidadeDisponivel = quantidade,
    };

    // Act
    await service.AdicionarProdutoAsync(produtoValido);

    // assert
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Once);
  }

  [Theory]
  [InlineData(-1)]
  [InlineData(-10000.0)]
  [InlineData(-0.1)]
  public async Task AdicionarProdutoAsync_DeveLancaExcecao_QuandoQuantidadeMenorQueZero(int quantidade)
  {
    // Arrange
    var mockRepo = new Mock<IProdutoRepository>();
    var service = new ProdutoService(mockRepo.Object);

    var produtoInvalido = new Produto
    {
      Id = Guid.NewGuid(),
      Nome = "Produto Teste",
      Preco = 10,
      QuantidadeDisponivel = quantidade
    };

    // Act
    var act = async () => await service.AdicionarProdutoAsync(produtoInvalido);

    // assert
    var execao = await Assert.ThrowsAsync<ArgumentException>(act);
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Never);
  }

  [Theory]
  [InlineData(-1)]
  [InlineData(-10000.0)]
  [InlineData(-0.1)]
  public async Task AdicionarProdutoAsync_DeveLancaExcecao_QuandoPrecoMenorQueZero(decimal preco)
  {
    // Arrange
    var mockRepo = new Mock<IProdutoRepository>();
    var service = new ProdutoService(mockRepo.Object);

    var produtoInvalido = new Produto
    {
      Id = Guid.NewGuid(),
      Nome = "Produto Teste",
      Preco = preco,
      QuantidadeDisponivel = 5,
    };

    // Act
    var act = async () => await service.AdicionarProdutoAsync(produtoInvalido);

    // assert
    var execao = await Assert.ThrowsAsync<ArgumentException>(act);
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Never);
  }

  [Theory]
  [InlineData("")]
  [InlineData(null)]
  public async Task AdicionarprodutoAsync_DeveLancaExcecao_QuandoNomeForNuloOuVazio(string nome)
  {
    var mockRepo = new Mock<IProdutoRepository>();
    var service = new ProdutoService(mockRepo.Object);
    var produtoInvalido = new Produto
    {
      Id = Guid.NewGuid(),
      Nome = nome,
      Preco = 10,
      QuantidadeDisponivel = 5,
    };

    var act = async () => await service.AdicionarProdutoAsync(produtoInvalido);

    var execao = await Assert.ThrowsAsync<ArgumentException>(act);
    mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Never);
  }

  [Fact]
  public async Task AtualizaProdutoAsync_DeveLancaExcecao_QuandoProdutoTentarAtualizarForIgualJaInserido()
  {
    var mockRepo = new Mock<IProdutoRepository>();
    var service = new ProdutoService(mockRepo.Object);
    var produtoInserido = new Produto
    {
      Id = Guid.NewGuid(),
      Nome = "Produto Igual Teste",
      Preco = 10,
      QuantidadeDisponivel = 5,
    };
    mockRepo.Setup(repo => repo.ObterPorIdAsync(produtoInserido.Id)).ReturnsAsync(produtoInserido);
    var produtoAtualizaRepetido = new Produto
    {
        Id = produtoInserido.Id,
        Nome = produtoInserido.Nome,
        Preco = 10,
        QuantidadeDisponivel = 5,
    };


    var act = async () => await service.AtualizarProdutoAsync(produtoAtualizaRepetido);

    var excecao = await Assert.ThrowsAsync<ArgumentException>(act);
    mockRepo.Verify(repo => repo.AtualizarAsync(produtoAtualizaRepetido), Times.Never);
  }
}
