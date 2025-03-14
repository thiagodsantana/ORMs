namespace CsharpORM.Domain.Classes
{
    public class Emprestimo
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public int Parcelas { get; set; }
        public decimal TaxaJuros { get; set; }
        public int ClienteId { get; set; }

        public virtual Cliente? Cliente { get; set; }
    }
}
