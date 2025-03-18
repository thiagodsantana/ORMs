using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CsharpORM.Domain.Classes
{
    public class Emprestimo
    {
        [Key] // Define como chave primária
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] // Define precisão do decimal no banco
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do empréstimo deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [Required]
        [Range(1, 360, ErrorMessage = "O número de parcelas deve estar entre 1 e 360.")]
        public int Parcelas { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")] // Percentual com duas casas decimais
        [Range(0.01, 100, ErrorMessage = "A taxa de juros deve estar entre 0.01% e 100%.")]
        public decimal TaxaJuros { get; set; }

        [Required]
        [ForeignKey("Cliente")] // Define chave estrangeira
        public int ClienteId { get; set; }

        public virtual Cliente? Cliente { get; set; } // Relacionamento com Cliente (Lazy Loading)
    }
}
