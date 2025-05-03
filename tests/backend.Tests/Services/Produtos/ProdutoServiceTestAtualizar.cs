using backend.Models;
using backend.Repository;
using backend.Services.Produtos;
using Moq;

namespace backend.Tests.Services.Produtos;

public class ProdutoServiceTestAtualizar
{
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
  [Theory]
  [InlineData("Produto diferente teste")]
  [InlineData("Produto 1.diferente teste")]
  [InlineData("Produto 1-diferente teste")]
  [InlineData("Produto 1.-diferente teste")]
  [InlineData("Produto 1-.diferente teste")]
  [InlineData("produto 1.diferente teste")]
  [InlineData("produto 1-diferente teste")]
  [InlineData("produto 1.-diferente teste")]
  [InlineData("produto 1-.diferente teste")]
  public async Task AtualizaProdutoAsync_DeveAtualizar_QuandoProdutoNomeForValidoEDiferente(string nome)
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
    var produtoAtualizarValido = new Produto
    {
        Id = produtoInserido.Id,
        Nome = nome,
        Preco = 10,
        QuantidadeDisponivel = 5,
    };


    await service.AtualizarProdutoAsync(produtoAtualizarValido);

    mockRepo.Verify(repo => repo.AtualizarAsync(produtoAtualizarValido), Times.Once);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(9.00)]
  [InlineData(999999.00)]
  [InlineData(999999.999)]
  [InlineData(10.99)]
  public async Task AtualizaProdutoAsync_DeveAtualizar_QuandoProdutoPrecoForValidoEDiferente(decimal preco)
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
    var produtoAtualizarValido = new Produto
    {
        Id = produtoInserido.Id,
        Nome = produtoInserido.Nome,
        Preco = preco,
        QuantidadeDisponivel = 5,
    };


    await service.AtualizarProdutoAsync(produtoAtualizarValido);

    mockRepo.Verify(repo => repo.AtualizarAsync(produtoAtualizarValido), Times.Once);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(9.00)]
  [InlineData(999999.00)]
  [InlineData(999999.999)]
  [InlineData(10.99)]
  public async Task AtualizaProdutoAsync_DeveAtualizar_QuandoProdutoQuantidadeForValidoEDiferente(int quantidade)
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
    var produtoAtualizarValido = new Produto
    {
        Id = produtoInserido.Id,
        Nome = produtoInserido.Nome,
        Preco = produtoInserido.Preco,
        QuantidadeDisponivel = quantidade,
    };


    await service.AtualizarProdutoAsync(produtoAtualizarValido);

    mockRepo.Verify(repo => repo.AtualizarAsync(produtoAtualizarValido), Times.Once);
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

  [Theory]
  [InlineData("")]
  [InlineData(null)]
  public async Task AtualizaProdutoAsync_DeveLancaExcecao_QuandoProdutoTentarAtualizarTerNomeNuloOuVazio(string nome)
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
    var produtoInvalido = new Produto
    {
        Id = produtoInserido.Id,
        Nome = nome,
        Preco = 10,
        QuantidadeDisponivel = 5,
    };


    var act = async () => await service.AtualizarProdutoAsync(produtoInvalido);

    var excecao = await Assert.ThrowsAsync<ArgumentException>(act);
    mockRepo.Verify(repo => repo.AtualizarAsync(produtoInvalido), Times.Never);
  }

  [Theory]
  [InlineData("1.Ola")]
  [InlineData("1Ola")]
  [InlineData(".1Ola")]
  [InlineData(".Ola1")]
  public async Task AtualizaProdutoAsync_DeveLancaExcecao_QuandoProdutoTentarAtualizarTerNomeSemComecaPorLetras(string nome)
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
    var produtoInvalido = new Produto
    {
        Id = produtoInserido.Id,
        Nome = nome,
        Preco = 10,
        QuantidadeDisponivel = 5,
    };


    var act = async () => await service.AtualizarProdutoAsync(produtoInvalido);

    var excecao = await Assert.ThrowsAsync<ArgumentException>(act);
    mockRepo.Verify(repo => repo.AtualizarAsync(produtoInvalido), Times.Never);
  }

  [Theory]
  [InlineData(1000001.00)]
  [InlineData(1000000.01)]
  [InlineData(-0.01)]
  public async Task AtualizaProdutoAsync_DeveLancaExcecao_QuandoProdutoTentarAtualizarPrecoForaDosLimites(decimal preco)
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
    var produtoInvalido = new Produto
    {
        Id = produtoInserido.Id,
        Nome = produtoInserido.Nome,
        Preco = preco,
        QuantidadeDisponivel = 5,
    };


    var act = async () => await service.AtualizarProdutoAsync(produtoInvalido);

    var excecao = await Assert.ThrowsAsync<ArgumentException>(act);
    mockRepo.Verify(repo => repo.AtualizarAsync(produtoInvalido), Times.Never);
  }

  [Theory]
  [InlineData(1000001)]
  [InlineData(-1)]
  public async Task AtualizaProdutoAsync_DeveLancaExcecao_QuandoProdutoTentarAtualizarQuantidadeForaDosLimites(int quantidade)
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
    var produtoInvalido = new Produto
    {
        Id = produtoInserido.Id,
        Nome = produtoInserido.Nome,
        Preco = produtoInserido.Preco,
        QuantidadeDisponivel = quantidade,
    };


    var act = async () => await service.AtualizarProdutoAsync(produtoInvalido);

    var excecao = await Assert.ThrowsAsync<ArgumentException>(act);
    mockRepo.Verify(repo => repo.AtualizarAsync(produtoInvalido), Times.Never);
  }
}
