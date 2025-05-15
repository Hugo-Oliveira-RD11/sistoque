using backend.Models;
using backend.Services.Produtos;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using backend.Repository.Produtos;

namespace backend.Tests.Services.Produtos;

public class ProdutoServiceTestRemover
{
    private readonly Mock<IProdutoRepository> _mockRepo;
    private readonly Mock<IValidator<Produto>> _mockValidator;
    private readonly Mock<IHttpContextAccessor> _mockContextAccessor;
    private readonly ProdutoService _service;
    private readonly Guid _usuarioId = Guid.NewGuid();
    private readonly Guid _outroUsuarioId = Guid.NewGuid();

    public ProdutoServiceTestRemover()
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

    private Produto CriarProdutoPadrao(
        Guid? id = null,
        Guid? usuarioId = null,
        string nome = "Produto Teste",
        decimal preco = 10.0M,
        int quantidade = 5)
    {
        return new Produto
        {
            Id = id ?? Guid.NewGuid(),
            usuarioId = usuarioId ?? _usuarioId,
            Nome = nome,
            Preco = preco,
            QuantidadeDisponivel = quantidade
        };
    }

    [Fact]
    public async Task RemoverProdutoAsync_DeveRemover_QuandoExistirEUsuarioForDono()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtoRemover = CriarProdutoPadrao(id);

        _mockRepo.Setup(repo => repo.ObterPorIdAsync(id))
            .ReturnsAsync(produtoRemover);

        // Act
        await _service.RemoverProdutoAsync(id);

        // Assert
        _mockRepo.Verify(repo => repo.ObterPorIdAsync(id), Times.Once);
        _mockRepo.Verify(repo => repo.RemoverAsync(id), Times.Once);
    }

    [Theory]
    [InlineData("08bb54f0-2a6d-4a88-ba16-1c0bc9fb5766")] // id inexistente
    [InlineData("1a9f04a8-2d1a-485a-b2fd-3fec29aafbff")]
    public async Task RemoverProdutoAsync_DeveLancarExcecao_QuandoNaoExistir(string stringId)
    {
        // Arrange
        var id = Guid.Parse(stringId);
        _mockRepo.Setup(repo => repo.ObterPorIdAsync(id))
            .ReturnsAsync((Produto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.RemoverProdutoAsync(id));

        _mockRepo.Verify(repo => repo.ObterPorIdAsync(id), Times.Once);
        _mockRepo.Verify(repo => repo.RemoverAsync(id), Times.Never);
    }

    [Fact]
    public async Task RemoverProdutoAsync_DeveLancarExcecao_QuandoUsuarioNaoForDono()
    {
        // Arrange
        var id = Guid.NewGuid();
        var produtoDeOutroUsuario = CriarProdutoPadrao(id, _outroUsuarioId);

        _mockRepo.Setup(repo => repo.ObterPorIdAsync(id))
            .ReturnsAsync(produtoDeOutroUsuario);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.RemoverProdutoAsync(id));

        _mockRepo.Verify(repo => repo.ObterPorIdAsync(id), Times.Once);
        _mockRepo.Verify(repo => repo.RemoverAsync(id), Times.Never);
    }

    [Fact]
    public async Task RemoverProdutoAsync_DeveLancarExcecao_QuandoUsuarioNaoAutenticado()
    {
        // Arrange
        var serviceSemUsuario = new ProdutoService(
            _mockRepo.Object,
            _mockValidator.Object,
            new Mock<IHttpContextAccessor>().Object); // Contexto sem usuário

        var id = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => serviceSemUsuario.RemoverProdutoAsync(id));

        _mockRepo.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockRepo.Verify(repo => repo.RemoverAsync(It.IsAny<Guid>()), Times.Never);
    }
}
