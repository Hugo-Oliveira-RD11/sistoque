namespace backend.Services.Clientes;

public interface IHashServices
{
  string GerarHashSenha(string senha);
  bool VerificarSenha(string senha, string senhaHash);
}
