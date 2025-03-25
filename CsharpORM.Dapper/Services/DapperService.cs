using CsharpORM.Domain.Classes;
using Dapper;
using System.Data;

namespace CsharpORM.Dapper.Services
{
    // Serviço que encapsula operações com o Dapper para manipulação de
    // clientes e empréstimos.
    public class DapperService(IDbConnection dbConnection)
    {
        #region Query

        /// <summary>
        /// Busca todos os clientes cadastrados na tabela "Clientes".
        /// </summary>
        public async Task<IEnumerable<Cliente>> GetClientesAsync()
        {
            // Consulta SQL que seleciona as colunas de interesse na tabela Clientes.
            const string sqlClientes = @"SELECT c.Id, c.Nome, c.Cpf
                                         FROM Clientes c";

            // Executa a consulta de forma assíncrona e
            // mapeia o resultado para a classe Cliente.
            var clientes = await dbConnection.QueryAsync<Cliente>(sqlClientes);
            return clientes;
        }

        #endregion

        #region MultiMapping
        /// <summary>
        /// Busca os clientes e seus respectivos empréstimos utilizando mapeamento de relacionamento (1:N).
        /// </summary>
        public async Task<IEnumerable<Cliente>> GetClientesComEmprestimos()
        {
            // Consulta SQL com LEFT JOIN para retornar clientes e seus empréstimos.
            const string sql = @"
                SELECT 
                    c.Id, c.Nome, c.Cpf, 
                    e.Id AS EmprestimoId, e.Valor, e.Parcelas, e.TaxaJuros, e.ClienteId
                FROM Clientes c
                INNER JOIN Emprestimos e ON e.ClienteId = c.Id";

            // Dicionário para armazenar os clientes já mapeados, evitando duplicações.
            var clientes = new Dictionary<int, Cliente>();

            // Executa a consulta utilizando multi-mapping para associar
            // cada cliente aos seus empréstimos.
            await dbConnection.QueryAsync<Cliente, Emprestimo, Cliente>(
                sql,
                (cliente, emprestimo) =>
                {
                    // Se o cliente ainda não foi adicionado, inicializa sua lista de empréstimos.
                    if (!clientes.TryGetValue(cliente.Id, out var clienteExistente))
                    {
                        clienteExistente = cliente;
                        clienteExistente.Emprestimos = [];
                        clientes.Add(clienteExistente.Id, clienteExistente);
                    }

                    // Se houver um empréstimo válido, adiciona-o à lista do cliente.
                    // Adiciona o empréstimo à lista do cliente, se for válido.
                    if (emprestimo?.Id > 0)
                    {
                        clienteExistente.Emprestimos!.Add(emprestimo);
                    }
                    return clienteExistente;
                },
                splitOn: "EmprestimoId"
            );

            // Retorna os clientes com seus respectivos empréstimos.
            return clientes.Values;
        }
        #endregion

        #region Query Multiple
        /// <summary>
        /// Busca todos os clientes cadastrados na tabela "Clientes" e retorna a contagem total.
        /// </summary>
        /// <returns>Uma tupla contendo o total de clientes e a lista de clientes.</returns>
        public async Task<(int totalCount, IEnumerable<Cliente> clientes)> GetClientesCountAsync()
        {
            // Consulta SQL que retorna a contagem total de clientes e a lista de clientes.
            const string sql = @"SELECT COUNT(*) FROM Clientes; 
                         SELECT c.Id, c.Nome, c.Cpf FROM Clientes c;";

            // Executa a consulta e processa múltiplos resultados.
            using var multi = await dbConnection.QueryMultipleAsync(sql);

            // Lê o primeiro resultado como a contagem total de clientes.
            int totalCount = await multi.ReadFirstAsync<int>();

            // Lê o segundo resultado como a lista de clientes.
            var clientes = await multi.ReadAsync<Cliente>();

            // Retorna a contagem total e a lista de clientes.
            return (totalCount, clientes);
        }
        #endregion

        #region Transação
        /// <summary>
        /// Insere um novo empréstimo na tabela "Emprestimos" dentro de uma transação local.
        /// </summary>
        public async Task CriarEmprestimo(Emprestimo emprestimo)
        {
            const string sql = @"
                INSERT INTO Emprestimos (Valor, Parcelas, TaxaJuros, ClienteId) 
                VALUES (@Valor, @Parcelas, @TaxaJuros, @ClienteId)";

            // Inicia uma transação local utilizando o método BeginTransaction da conexão.
            using var transaction = dbConnection.BeginTransaction();
            try
            {
                // Executa a inserção do empréstimo dentro da transação.
                await dbConnection.ExecuteAsync(sql, emprestimo, transaction);
                // Se tudo ocorrer bem, confirma (commita) a transação.
                transaction.Commit();
            }
            catch
            {
                // Em caso de erro, desfaz (rollback) a transação e relança a exceção.
                transaction.Rollback();
                throw;
            }
        }        
        #endregion

        #region Stored Procedure
        /// <summary>
        /// Atualiza os dados de um cliente e insere um novo empréstimo em uma única transação
        /// utilizando uma stored procedure.
        /// </summary>
        public async Task AtualizarClienteStoredProcedure(Cliente cliente, Emprestimo emprestimo)
        {
            const string procedureName = "sp_AtualizarClienteEInserirEmprestimo";

            var parametros = new
            {
                ClienteId = cliente.Id,
                cliente.Nome,
                cliente.Cpf,
                emprestimo.Valor,
                emprestimo.Parcelas,
                emprestimo.TaxaJuros
            };

            await dbConnection.ExecuteAsync(procedureName, parametros, commandType: CommandType.StoredProcedure);
        }
        #endregion
    }

}
