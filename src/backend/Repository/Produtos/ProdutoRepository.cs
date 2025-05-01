using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Produto>> ObterTodosAsync()
        => await _context.Produtos.ToListAsync();

    public async Task<Produto?> ObterPorIdAsync(Guid id)
        => await _context.Produtos.FindAsync(id);

    public async Task AdicionarAsync(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var produto = await ObterPorIdAsync(id);
        if (produto != null)
          throw new NullReferenceException("Produto inexitente");

        _context.Produtos.Remove(produto!);
        await _context.SaveChangesAsync();
    }
}
