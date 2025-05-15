using backend.Models;
using backend.Repository.Produtos;
using backend.Services.Produtos;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace backend.Tests.Services.Produtos;

public class ProdutoServiceTestAtualizar
{
    private readonly Mock<IProdutoRepository> _mockRepo;
    private readonly Mock<IValidator<Produto>> _mockValidator;
    private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
    private readonly ProdutoService _service;
    private readonly Guid _usuarioId = Guid.NewGuid();
    private readonly Guid _outroUsuarioId = Guid.NewGuid();

    public ProdutoServiceTestAtualizar()
    {
        _mockRepo = new Mock<IProdutoRepository>();
        _mockValidator = new Mock<IValidator<Produto>>();
        _mockContextAccessor = new Mock<IHttpContextAccessor>();

        // Configuração padrão do contexto HTTP com usuário autenticado
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, _usuarioId.ToString())
        }));

        _mockContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = user
        });

        _service = new ProdutoService(_mockRepo.Object, _mockValidator.Object, _mockContextAccessor.Object);
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
        Guid? usuarioId = null,
        string nome = "Produto Teste",
        decimal preco = 10.0M,
        int quantidade = 5,
        string descricao = null,
        DateTime? dataValidade = null)
    {
        return new Produto
        {
            Id = id ?? Guid.NewGuid(),
            usuarioId = usuarioId ?? _usuarioId,
            Nome = nome,
            Preco = preco,
            QuantidadeDisponivel = quantidade,
            Descricao = descricao,
            DataValidade = dataValidade
        };
    }

    private void SetupProdutoExistente(Produto produto)
    {
        _mockRepo.Setup(repo => repo.ObterPorIdAsync(produto.Id))
                .ReturnsAsync(produto);
    }


    [Fact]
    public async Task AtualizarProdutoAsync_DeveLancarExcecao_QuandoProdutoForIgual()
    {
        // Arrange
        SetupValidatorSuccess();
        var produtoExistente = CriarProdutoPadrao();
        SetupProdutoExistente(produtoExistente);

        var produtoAtualizado = CriarProdutoPadrao(
            id: produtoExistente.Id,
            nome: produtoExistente.Nome,
            preco: produtoExistente.Preco,
            quantidade: produtoExistente.QuantidadeDisponivel);

        // Act
        var act = async () => await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
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
    public async Task AtualizarProdutoAsync_DeveAtualizar_QuandoNomeForValidoEDiferente(string nome)
    {
        // Arrange
        SetupValidatorSuccess();
        var produtoExistente = CriarProdutoPadrao();
        SetupProdutoExistente(produtoExistente);

        var produtoAtualizado = CriarProdutoPadrao(
            id: produtoExistente.Id,
            nome: nome,
            preco: produtoExistente.Preco,
            quantidade: produtoExistente.QuantidadeDisponivel);

        // Act
        await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        _mockRepo.Verify(repo => repo.AtualizarAsync(produtoAtualizado), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9.00)]
    [InlineData(999999.00)]
    [InlineData(999999.999)]
    [InlineData(10.99)]
    public async Task AtualizarProdutoAsync_DeveAtualizar_QuandoPrecoForValidoEDiferente(decimal preco)
    {
        // Arrange
        SetupValidatorSuccess();
        var produtoExistente = CriarProdutoPadrao();
        SetupProdutoExistente(produtoExistente);

        var produtoAtualizado = CriarProdutoPadrao(
            id: produtoExistente.Id,
            nome: produtoExistente.Nome,
            preco: preco,
            quantidade: produtoExistente.QuantidadeDisponivel);

        // Act
        await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        _mockRepo.Verify(repo => repo.AtualizarAsync(produtoAtualizado), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    [InlineData(999999)]
    [InlineData(10)]
    public async Task AtualizarProdutoAsync_DeveAtualizar_QuandoQuantidadeForValidoEDiferente(int quantidade)
    {
        // Arrange
        SetupValidatorSuccess();
        var produtoExistente = CriarProdutoPadrao();
        SetupProdutoExistente(produtoExistente);

        var produtoAtualizado = CriarProdutoPadrao(
            id: produtoExistente.Id,
            nome: produtoExistente.Nome,
            preco: produtoExistente.Preco,
            quantidade: quantidade);

        // Act
        await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        _mockRepo.Verify(repo => repo.AtualizarAsync(produtoAtualizado), Times.Once);
    }


    [Fact]
    public async Task AtualizarProdutoAsync_DeveLancarExcecao_QuandoUsuarioNaoForDono()
    {
        // Arrange
        SetupValidatorSuccess();
        var produtoExistente = CriarProdutoPadrao(usuarioId: _outroUsuarioId);
        SetupProdutoExistente(produtoExistente);

        var produtoAtualizado = CriarProdutoPadrao(
            id: produtoExistente.Id,
            nome: "Novo nome",
            preco: produtoExistente.Preco,
            quantidade: produtoExistente.QuantidadeDisponivel);

        // Act
        var act = async () => await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarProdutoAsync_DeveLancarExcecao_QuandoProdutoNaoExistir()
    {
        // Arrange
        SetupValidatorSuccess();
        var id = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.ObterPorIdAsync(id))
                .ReturnsAsync((Produto?)null);

        var produtoAtualizado = CriarProdutoPadrao(id: id);

        // Act
        var act = async () => await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task AtualizarProdutoAsync_DeveLancarExcecao_QuandoNomeForNuloOuVazio(string nome)
    {
        // Arrange
        SetupValidatorFailure("Nome é obrigatório");
        var produtoExistente = CriarProdutoPadrao();
        SetupProdutoExistente(produtoExistente);

        var produtoAtualizado = CriarProdutoPadrao(
            id: produtoExistente.Id,
            nome: nome,
            preco: produtoExistente.Preco,
            quantidade: produtoExistente.QuantidadeDisponivel);

        // Act
        var act = async () => await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Theory]
    [InlineData("1.Ola")]
    [InlineData("1Ola")]
    [InlineData(".1Ola")]
    [InlineData(".Ola1")]
    public async Task AtualizarProdutoAsync_DeveLancarExcecao_QuandoNomeComecarDiferenteDeLetras(string nome)
    {
        // Arrange
        SetupValidatorFailure("Nome deve começar com letras");
        var produtoExistente = CriarProdutoPadrao();
        SetupProdutoExistente(produtoExistente);

        var produtoAtualizado = CriarProdutoPadrao(
            id: produtoExistente.Id,
            nome: nome,
            preco: produtoExistente.Preco,
            quantidade: produtoExistente.QuantidadeDisponivel);

        // Act
        var act = async () => await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Theory]
    [InlineData(1000001.00)]
    [InlineData(1000000.01)]
    [InlineData(-0.01)]
    public async Task AtualizarProdutoAsync_DeveLancarExcecao_QuandoPrecoForaDosLimites(decimal preco)
    {
        // Arrange
        SetupValidatorFailure("Preço fora dos limites permitidos");
        var produtoExistente = CriarProdutoPadrao();
        SetupProdutoExistente(produtoExistente);

        var produtoAtualizado = CriarProdutoPadrao(
            id: produtoExistente.Id,
            nome: produtoExistente.Nome,
            preco: preco,
            quantidade: produtoExistente.QuantidadeDisponivel);

        // Act
        var act = async () => await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Theory]
    [InlineData(1000001)]
    [InlineData(-1)]
    public async Task AtualizarProdutoAsync_DeveLancarExcecao_QuandoQuantidadeForaDosLimites(int quantidade)
    {
        // Arrange
        SetupValidatorFailure("Quantidade fora dos limites permitidos");
        var produtoExistente = CriarProdutoPadrao();
        SetupProdutoExistente(produtoExistente);

        var produtoAtualizado = CriarProdutoPadrao(
            id: produtoExistente.Id,
            nome: produtoExistente.Nome,
            preco: produtoExistente.Preco,
            quantidade: quantidade);

        // Act
        var act = async () => await _service.AtualizarProdutoAsync(produtoAtualizado);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
        _mockRepo.Verify(repo => repo.AtualizarAsync(It.IsAny<Produto>()), Times.Never);
    }

    }
