namespace backend.Repository.Token;

using System.Collections.Concurrent;

public class InMemoryTokenRepository : ITokenRepository
{
  private readonly ConcurrentDictionary<string, bool> _tokens = new();

  public void Salvar(string token)
  {
    _tokens[token] = true;
  }

  public void Remover(string token)
  {
    _tokens.TryRemove(token, out _);
  }

  public bool Existe(string token)
  {
    return _tokens.ContainsKey(token);
  }

  public Task SalvarAsync(string token)
  {
    Salvar(token);
    return Task.CompletedTask;
  }

  public Task RemoverAsync(string token)
  {
    Remover(token);
    return Task.CompletedTask;
  }

  public Task<bool> ExisteAsync(string token)
  {
    var existe = Existe(token);
    return Task.FromResult(existe);
  }
}


