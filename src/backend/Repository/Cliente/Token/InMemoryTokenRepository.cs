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
}


