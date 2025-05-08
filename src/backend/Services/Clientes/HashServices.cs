namespace backend.Services.Clientes;

public class HashService : IHashServices
{
  private readonly int _cost;

  public HashService(IConfiguration configuration)
  {
    _cost = int.Parse(configuration["BCrypt:Cost"] ?? "10");
  }
  public int Cost() => _cost;

    public string GerarHashSenha(string senha) => BCrypt.Net.BCrypt.HashPassword(senha, workFactor: _cost);

    public bool VerificarSenha(string senha, string senhaHash) => BCrypt.Net.BCrypt.Verify(senha, senhaHash);
}
