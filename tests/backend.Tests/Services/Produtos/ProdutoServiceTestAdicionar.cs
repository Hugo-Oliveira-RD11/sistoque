using backend.Models;
using backend.Repository.Produtos;
using backend.Services.Produtos;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace backend.Tests.Services.Produtos;

public class ProdutoServiceTestAdicionar
{
    private readonly Mock<IProdutoRepository> _mockRepo;
    private readonly Mock<IValidator<Produto>> _mockValidator;
    private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
    private readonly ProdutoService _service;

    public ProdutoServiceTestAdicionar()
    {
        _mockRepo = new Mock<IProdutoRepository>();
        _mockValidator = new Mock<IValidator<Produto>>();
        _mockContextAccessor = new Mock<IHttpContextAccessor>();
        _service = new ProdutoService(_mockRepo.Object, _mockValidator.Object,_mockContextAccessor.Object);
    }

    private void SetupValidatorSuccess()
    {
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Produto>(), default))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidatorFailure(string errorMessage)
    {
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Property", errorMessage)
        };
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Produto>(), default))
            .ReturnsAsync(new ValidationResult(failures));
    }

    private Produto CriarProdutoPadrao(
        Guid? id = null,
        string nome = "Produto Teste",
        decimal preco = 10.0M,
        int quantidade = 5)
    {
        return new Produto
        {
            Id = id ?? Guid.NewGuid(),
            Nome = nome,
            Preco = preco,
            QuantidadeDisponivel = quantidade
        };
    }

    [Fact]
    public async Task AdicionarProdutoAsync_DeveAdicionarProduto_QuandoForValido()
    {
        // Arrange
        SetupValidatorSuccess();
        var produtoValido = CriarProdutoPadrao();

        // Act
        await _service.AdicionarProdutoAsync(produtoValido);

        // Assert
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Once);
        _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<Produto>(), default), Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 10.99)]
    [InlineData(10, 0.0)]
    [InlineData(0, 0.0)]
    public async Task AdicionarProdutoAsync_DeveAdicionarProduto_QuandoPrecoOuQuantidadeForZero(int quantidade, decimal preco)
    {
        // Arrange
        SetupValidatorSuccess();
        var produtoValido = CriarProdutoPadrao(preco: preco, quantidade: quantidade);

        // Act
        await _service.AdicionarProdutoAsync(produtoValido);

        // Assert
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Once);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10000)]
    public async Task AdicionarProdutoAsync_DeveLancarExcecao_QuandoQuantidadeMenorQueZero(int quantidade)
    {
        // Arrange
        SetupValidatorFailure("Quantidade não pode ser negativa");
        var produtoInvalido = CriarProdutoPadrao(quantidade: quantidade);

        // Act
        var act = async () => await _service.AdicionarProdutoAsync(produtoInvalido);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10000.0)]
    [InlineData(-0.1)]
    public async Task AdicionarProdutoAsync_DeveLancarExcecao_QuandoPrecoMenorQueZero(decimal preco)
    {
        // Arrange
        SetupValidatorFailure("Preço não pode ser negativo");
        var produtoInvalido = CriarProdutoPadrao(preco: preco);

        // Act
        var act = async () => await _service.AdicionarProdutoAsync(produtoInvalido);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task AdicionarProdutoAsync_DeveLancarExcecao_QuandoNomeForNuloOuVazio(string nome)
    {
        // Arrange
        SetupValidatorFailure("Nome é obrigatório");
        var produtoInvalido = CriarProdutoPadrao(nome: nome);

        // Act
        var act = async () => await _service.AdicionarProdutoAsync(produtoInvalido);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Theory]
    [InlineData("1.Ola")]
    [InlineData("1Ola")]
    [InlineData(".1Ola")]
    [InlineData(".Ola1")]
    public async Task AdicionarProdutoAsync_DeveLancarExcecao_QuandoNomeComecarDiferenteDeLetras(string nome)
    {
        // Arrange
        SetupValidatorFailure("Nome deve começar com letras");
        var produtoInvalido = CriarProdutoPadrao(nome: nome);

        // Act
        var act = async () => await _service.AdicionarProdutoAsync(produtoInvalido);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _mockRepo.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Never);
    }
}
