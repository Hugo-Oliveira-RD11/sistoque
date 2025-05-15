using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Models.Cliente>> ObterTodosAsync()
        => await _context.Clientes.ToListAsync();

    public async Task<Models.Cliente?> ObterIdAsync(Guid id)
        => await _context.Clientes.FindAsync(id);

    public async Task<Models.Cliente?> ObterEmailAsync(string email)
        => await _context.Clientes.FirstOrDefaultAsync(e => e.Email == email);

    public async Task<Models.Cliente?> ObterCpfCnpjAsync(string cpfCnpj)
        => await _context.Clientes.FirstOrDefaultAsync(e => e.CpfCnpj == cpfCnpj);

    public async Task AdicionarAsync(Models.Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Models.Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var cliente = await ObterIdAsync(id);
        _context.Clientes.Remove(cliente!); // nao deve precisar de verificar se o cliente e nulo pois o service vai fazer isso
        await _context.SaveChangesAsync();
    }

}
