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
          .Length(3, 255).WithMessage("o nome deve ter de 3 a 255 caracteres")
          .Matches(@"^[A-Za-z .]*$").WithMessage("nome nao deve ter numero ou outros caracteres especiais alem do ponto");

        RuleFor(c => c.CpfCnpj)
         .NotEmpty()
         .NotNull().WithMessage("O CPF ou CNPJ e obrigatorio.")
         .Matches(@"^(\d{11}|\d{14})$").WithMessage("CPF ou CNPJ deve conter apenas numeros com 11 ou 14 digitos.");

        RuleFor(c => c.Telefone)
          .NotNull()
          .Matches(@"^\d{11}$").WithMessage("telefone deve conter apenas numeros com o DDD (11 numeros)");

        RuleFor(c => c.SenhaHash)
          .NotEmpty().WithMessage("Senha nao pode ser vazia")
          .Length(6,60).WithMessage("senha deve ter entre 6 a 255 caracteres");

        RuleFor(c => c.Role)
          .NotEmpty().WithMessage("Nao deve ter uma Role nula ou vazia")
          .Must(role => role == "admin" || role == "user").WithMessage("A Role deve ser 'admin' ou 'user'");

        RuleFor(c => c.Email)
          .NotEmpty().WithMessage("Email nao pode ser vazio")
          .EmailAddress().WithMessage("O email Informado nao e valido");

    }
}
