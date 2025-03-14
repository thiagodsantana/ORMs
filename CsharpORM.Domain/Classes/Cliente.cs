using CsharpORM.Domain.Classes;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;
    public string Cpf { get; set; } = default!;

    public virtual List<Emprestimo>? Emprestimos { get; set; } // Lazy Loading precisa de "virtual"
}