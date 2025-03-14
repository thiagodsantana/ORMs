using CsharpORM.Domain.Classes;
using Dapper;
using System.Data;

namespace CsharpORM.Dapper.Services
{
    public class DapperService(IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<IEnumerable<Cliente>> GetClientes()
        {

            var sql = @"
        SELECT c.Id, c.Nome, c.Cpf, 
               e.Id, e.Valor, e.Parcelas, e.TaxaJuros, e.ClienteId
        FROM Clientes c
        LEFT JOIN Emprestimos e ON e.ClienteId = c.Id";

            var clientes = new Dictionary<int, Cliente>();

            var resultado = await _dbConnection.QueryAsync<Cliente, Emprestimo, Cliente>(
                sql,
                (cliente, emprestimo) =>
                {
                    if (!clientes.TryGetValue(cliente.Id, out var clienteExistente))
                    {
                        clienteExistente = cliente;
                        clienteExistente.Emprestimos = new List<Emprestimo>();
                        clientes.Add(clienteExistente.Id, clienteExistente);
                    }

                    if (emprestimo != null)
                    {
                        clienteExistente.Emprestimos.Add(emprestimo);
                    }

                    return clienteExistente;
                },
                splitOn: "Id"
            );

            return clientes.Values;
        }


        public async Task CriarEmprestimo(Emprestimo emprestimo)
        {
            const string sql = "INSERT INTO Emprestimos (Valor, Parcelas, TaxaJuros, ClienteId) VALUES (@Valor, @Parcelas, @TaxaJuros, @ClienteId)";

            if (_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                await _dbConnection.ExecuteAsync(sql, emprestimo, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw; // Repassa a exceção para ser tratada na camada superior
            }
        }
    }
}
