using Dapper;
using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public class AccountRepository: IAccountRepository
    {
        private readonly string _connectionString;
        public AccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Account account)
        {
            using var connection = new SqlConnection(_connectionString);

            var id = await connection.QuerySingleAsync<int>(
                        @"INSERT INTO Accounts (Name, AccountTypeId, Description, Balance)
                            VALUES (@Name, @AccountTypeId, @Description, @Balance);
                             SELECT SCOPE_IDENTITY();", account);

            account.Id = id;
        }

        public async Task<IEnumerable<Account>> Search(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Account>(@"SELECT Accounts.Id, Accounts.Name, Accounts.Balance, at.Name AS AccountType
                                                            FROM Accounts
                                                            INNER JOIN AccountTypes at
                                                            on at.Id = Accounts.AccountTypeId
                                                            WHERE at.UserId = @UserId
                                                            ORDER BY at.[Order]", new {userId});
        }

        public async Task<Account> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>(@"SELECT Accounts.Id, Accounts.Name, Accounts.Balance, Description, Accounts.AccountTypeId
                                                                        FROM Accounts
                                                                        INNER JOIN AccountTypes at
                                                                        on at.Id = Accounts.AccountTypeId
                                                                        WHERE at.UserId = @UserId AND Accounts.Id = @Id", new { id, userId });
        }

        public async Task Update(AccountCreationViewModel account)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(@"Update Accounts
                                            SET Name = @Name, Balance = @Balance, Description = @Description,
                                            AccountTypeId = @AccountTypeId
                                            WHERE Id = @Id;", account);

        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("DELETE Accounts WHERE Id = @Id", new { id });
        }

    }
}
