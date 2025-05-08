public interface ITokenRepository
{
  void Salvar(string token);
  void Remover(string token);
  bool Existe(string token);
}
