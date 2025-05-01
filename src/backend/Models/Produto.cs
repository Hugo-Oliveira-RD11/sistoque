namespace backend.Models;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeDisponivel { get; set; }

    public string? Descricao { get; set; }
    public DateTime? DataValidade { get; set; }
}
