public interface ITokenRepository
{
  void Salvar(string token);
  void Remover(string token);
  bool Existe(string token);
  Task SalvarAsync(string token);
  Task RemoverAsync(string token);
  Task<bool> ExisteAsync(string token);
}
