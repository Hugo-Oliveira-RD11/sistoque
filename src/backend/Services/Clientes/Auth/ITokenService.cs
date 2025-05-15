using backend.Models;

namespace backend.Services.Clientes.Auth;
public interface ITokenService
{
  string GerarToken(Cliente cliente);
  Task SalvarToken(string token);
  Task RemoverToken(string token);
  Task<bool> TokenValido(string token);
}
