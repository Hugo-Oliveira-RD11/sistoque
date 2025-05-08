using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services.Clientes.Auth;

public class TokenService : ITokenService
{
  private readonly IConfiguration _configuration;
  private readonly ITokenRepository _tokenRepository;

  public TokenService(IConfiguration configuration, ITokenRepository tokenRepository)
  {
    _configuration = configuration;
    _tokenRepository = tokenRepository;
  }

  public string GerarToken(Cliente cliente)
  {
    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);
    var tokenHandler = new JwtSecurityTokenHandler();

    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
      new Claim(ClaimTypes.Email, cliente.Email!),
      new Claim(ClaimTypes.Role, cliente.Role!)
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddHours(2),
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  public void SalvarToken(string token)
  {
    _tokenRepository.Salvar(token);
  }

  public void RemoverToken(string token)
  {
    _tokenRepository.Remover(token);
  }

  public bool TokenValido(string token)
  {
    return _tokenRepository.Existe(token);
  }
}


