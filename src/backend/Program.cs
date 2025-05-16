using System.Text;
using backend.Data;
using backend.Models;
using backend.Repository;
using backend.Repository.Produtos;
using backend.Repository.Token;
using backend.Services.Clientes;
using backend.Services.Clientes.Auth;
using backend.Services.Produtos;
using backend.Validacao;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey não configurada");
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(options =>
  {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = issuer,
      ValidAudience = audience,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
      ClockSkew = TimeSpan.Zero
    };
  });

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
  options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
});

builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlite("Data Source=meuDB.db"));

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddSingleton<IHashServices, HashService>();
builder.Services.AddSingleton<ITokenRepository,InMemoryTokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IValidator<Cliente>, ValidarCliente>();
builder.Services.AddScoped<IValidator<Produto>, ValidarProdutos>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
