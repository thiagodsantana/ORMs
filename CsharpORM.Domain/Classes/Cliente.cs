using CsharpORM.Domain.Classes;
using System.ComponentModel.DataAnnotations;

public class Cliente
{
    [Key] // Define como chave primária
    public int Id { get; set; }

    [Required] // Campo obrigatório
    [StringLength(100, MinimumLength = 3)] // Define tamanho mínimo e máximo
    public string Nome { get; set; } = default!;

    [Required]
    [StringLength(11)] // CPF tem 11 caracteres
    [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos numéricos.")]
    public string Cpf { get; set; } = default!;

    public virtual List<Emprestimo>? Emprestimos { get; set; } // Lazy Loading precisa de "virtual"
}