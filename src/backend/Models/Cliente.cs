

using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class Cliente
{
    [Key]
    public Guid Id { get; set; }

    public string Nome { get; set; }

    [Required]
    public string CpfCnpj { get; set; }

    public string Endereco { get; set; }

    public string Telefone { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string senhaHash { get; set; }

    [Required]
    public string Role { get; set; } // Ex: "Admin", "Usuario", etc.
}
