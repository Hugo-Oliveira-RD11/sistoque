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
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IConfiguration configuration,
        ITokenRepository tokenRepository,
        ILogger<TokenService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _tokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GerarToken(Cliente cliente)
    {
        try
        {
            var secretKey = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Chave JWT não configurada");
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, cliente.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, cliente.Email!),
                new Claim(ClaimTypes.Role, cliente.Role!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID único para o token
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) // Data de emissão
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            SalvarToken(tokenString); // Automaticamente salva o token gerado
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar token JWT");
            throw;
        }
    }

    public async Task SalvarToken(string token)
    {
        try
        {
            await _tokenRepository.SalvarAsync(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar token");
            throw;
        }
    }

    public async Task RemoverToken(string token)
    {
        try
        {
            await _tokenRepository.RemoverAsync(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover token");
            throw;
        }
    }

    public async Task<bool> TokenValido(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            // Verifica primeiro se o token está na lista de revogados/armazenados
            var existeNoRepositorio = await _tokenRepository.ExisteAsync(token);
            if (!existeNoRepositorio)
                return false;

            // Valida a estrutura do token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar token");
            return false;
        }
    }
}
