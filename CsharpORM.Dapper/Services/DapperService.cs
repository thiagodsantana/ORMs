using CsharpORM.Domain.Classes;
using Dapper;
using System.Data;

namespace CsharpORM.Dapper.Services
{
    public class DapperService(IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<IEnumerable<Cliente>> GetClientesAsync()
        {
            const string sqlClientes = @"SELECT c.Id, c.Nome, c.Cpf
                                            FROM Clientes c";

            // Obtenha os clientes
            var clientes = await _dbConnection.QueryAsync<Cliente>(sqlClientes);

            return clientes;
        }

        public async Task<IEnumerable<Cliente>> GetClientesComEmprestimos()
        {
            const string sql = @"
                SELECT 
                    c.Id, c.Nome, c.Cpf, 
                    e.Id AS EmprestimoId, e.Valor, e.Parcelas, e.TaxaJuros, e.ClienteId
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
                        clienteExistente.Emprestimos = [];
                        clientes.Add(clienteExistente.Id, clienteExistente);
                    }

                    if (emprestimo is not null && emprestimo.Id > 0)
                    {
                        clienteExistente.Emprestimos.Add(emprestimo);
                    }

                    return clienteExistente;
                },
                splitOn: "EmprestimoId"
            );

            return clientes.Values;
        }

        public async Task CriarEmprestimo(Emprestimo emprestimo)
        {
            const string sql = @"
                INSERT INTO Emprestimos (Valor, Parcelas, TaxaJuros, ClienteId) 
                VALUES (@Valor, @Parcelas, @TaxaJuros, @ClienteId)";

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                await _dbConnection.ExecuteAsync(sql, emprestimo, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
