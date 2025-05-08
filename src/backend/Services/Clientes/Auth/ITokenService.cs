using backend.Models;

namespace backend.Services.Clientes.Auth;
public interface ITokenService
{
  string GerarToken(Cliente cliente);
  void SalvarToken(string token);
  void RemoverToken(string token);
  bool TokenValido(string token);
}
