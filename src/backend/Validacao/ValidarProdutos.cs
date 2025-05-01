
using backend.Models;
using FluentValidation;

namespace backend.Validacao;

public class ValidarProdutos : AbstractValidator<Produto>
{
  public ValidarProdutos()
  {
    RuleFor(p => p.Nome)
      .NotEmpty()
      .NotNull().WithMessage("Nome do produto nao pode ser vazio")
      .MinimumLength(2).WithMessage("Produto deve ter pelo menos 2 caracteres")
      .MaximumLength(255).WithMessage("Produto nao pode ser maior que 255 caracteres")
      .Matches("^[A-Za-z][A-Za-z0-9 .-]*$")
        .WithMessage("o nome so pode comecar com letra, pode ter infen e ponto");


    RuleFor(p => p.Preco)
      .GreaterThanOrEqualTo(0).PrecisionScale(10,3,true) // as vezes o produto e de graca
      .WithMessage("Preco nao pode ser menor que Zero")
      .LessThanOrEqualTo(1000000)
      .WithMessage("O preco nao deve ser maior que 1 Milhao");


    RuleFor(p => p.QuantidadeDisponivel)
      .GreaterThanOrEqualTo(0)
      .WithMessage("O estoque nao deve ser negativo")
      .LessThanOrEqualTo(1000000).WithMessage("A quantidade nao deve ser maior que 1 Milhao");

    RuleFor(p => p.Descricao)
      .Length(3, 1000).WithMessage("A descricao deve ter 3 a 1000 caracteres");

  }
}
