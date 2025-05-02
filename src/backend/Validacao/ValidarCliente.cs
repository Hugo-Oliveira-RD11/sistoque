using backend.Models;
using FluentValidation;

namespace backend.Validacao;


public class ValidarCliente : AbstractValidator<Cliente>
{
    public ValidarCliente()
    {
      RuleFor(c => c.Nome)
        .NotEmpty()
        .NotNull().WithMessage("O nome nao deve ser vazio ou nulo")
        .Length(3,255).WithMessage("o nome deve ter de 3 a 255 caracteres");

    }
}
