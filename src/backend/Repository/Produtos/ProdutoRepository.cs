using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository.Produtos;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Produto>> ObterTodosAsync()
        => await _context.Produtos.ToListAsync();

    public async Task<IEnumerable<Produto>> ObterPorUsuarioIdAsync(Guid usuarioId)
        => await _context.Produtos
            .Where(p => p.usuarioId == usuarioId)
            .ToListAsync();

    public async Task<Produto?> ObterPorIdAsync(Guid id)
        => await _context.Produtos.FindAsync(id);

    public async Task<Produto?> ObterPorIdEUsuarioIdAsync(Guid id, Guid usuarioId)
        => await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id && p.usuarioId == usuarioId);

    public async Task AdicionarAsync(Produto produto)
    {
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));

        if (produto.usuarioId == Guid.Empty)
            throw new ArgumentException("Produto deve ter um UsuarioId válido");

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Produto produto)
    {
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));

        if (produto.usuarioId == Guid.Empty)
            throw new ArgumentException("Produto deve ter um UsuarioId válido");

        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var produto = await ObterPorIdAsync(id);
        if (produto == null)
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado");

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverPorIdEUsuarioIdAsync(Guid id, Guid usuarioId)
    {
        var produto = await ObterPorIdEUsuarioIdAsync(id, usuarioId);
        if (produto == null)
            throw new KeyNotFoundException($"Produto não encontrado ou não pertence ao usuário");

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
    }
}
