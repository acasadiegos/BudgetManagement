using Dapper;
using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly string _connectionString;
        public TransactionsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Transaction transaction)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>("Transactions_Insert",
                new
                {
                    transaction.UserId,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategorieId,
                    transaction.AccountId,
                    transaction.Note
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaction.Id = id;
        }

        public async Task Update(Transaction transaction, decimal previousAmount, int previousAccountId)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync("Update_Transactions",
                new
                {
                    transaction.Id,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategorieId,
                    transaction.AccountId,
                    transaction.Note,
                    previousAmount,
                    previousAccountId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaction>> GetByAccountId(GetTransactionsByAccount model)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Transaction>(@"SELECT t.Id, t.Amount, t.TransactionDate, c.Name as Categorie,
                                                            a.Name as Account, c.OperationTypeId
                                                            FROM Transactions t
                                                            INNER JOIN Categories c
                                                            on c.Id = t.CategorieId
                                                            INNER JOIN Accounts a
                                                            ON a.Id = t.AccountId
                                                            WHERE t.AccountId = @AccountId
                                                            AND t.UserId = @UserId
                                                            AND t.TransactionDate BETWEEN @StartDate AND @EndDate ", model);
        }

        public async Task<IEnumerable<Transaction>> GetByUserId(GetTransactionsByUserParams model)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Transaction>(@"SELECT t.Id, t.Amount, t.TransactionDate, c.Name as Categorie,
                                                            a.Name as Account, c.OperationTypeId, t.Note
                                                            FROM Transactions t
                                                            INNER JOIN Categories c
                                                            on c.Id = t.CategorieId
                                                            INNER JOIN Accounts a
                                                            ON a.Id = t.AccountId
                                                            WHERE t.UserId = @UserId
                                                            AND t.TransactionDate BETWEEN @StartDate AND @EndDate 
                                                            ORDER BY t.TransactionDate DESC", model);
        }

        public async Task<Transaction> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaction>(@"SELECT Transactions.*, cat.OperationTypeId
                                                                            FROM Transactions
                                                                            INNER JOIN Categories cat
                                                                            ON cat.Id = Transactions.CategorieId
                                                                            WHERE Transactions.Id = @Id
                                                                            AND Transactions.UserId = @UserId", 
                                                                            new {id, userId});
        }

        public async Task<IEnumerable<GetByWeekResult>> GetByWeek(
            GetTransactionsByUserParams model)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<GetByWeekResult>(@"
                                    SELECT datediff(d, @startDate, t.TransactionDate) / 7 + 1 as Week,
                                    SUM(t.Amount) as Amount, cat.OperationTypeId
                                    FROM Transactions t
                                    INNER JOIN Categories cat
                                    ON cat.Id = t.CategorieId
                                    WHERE t.UserId = @userId
                                    AND t.TransactionDate Between @startDate and @endDate
                                    GROUP BY datediff(d, @startDate, t.TransactionDate) / 7, cat.OperationTypeId", model);
        }

        public async Task<IEnumerable<GetByMonthResult>> GetByMonth(int userId, int year)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<GetByMonthResult>(@"SELECT MONTH(t.TransactionDate) as Month,
                                                    SUM(t.Amount) as Amount, cat.OperationTypeId
                                                    FROM Transactions t
                                                    INNER JOIN Categories cat
                                                    ON cat.Id = t.CategorieId
                                                    WHERE t.UserId = @userId
                                                    AND YEAR(t.TransactionDate) = @Year
                                                    GROUP BY Month(t.TransactionDate), cat.OperationTypeId;", new { userId, year });
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("Delete_Transactions",
                new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
