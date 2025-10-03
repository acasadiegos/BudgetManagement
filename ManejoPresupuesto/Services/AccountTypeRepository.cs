using Dapper;
using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public class AccountTypeRepository : IAccountTypeRepository
    {
        private readonly string _connectionString;
        public AccountTypeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(AccountType accountType)
        {
            using var connection = new SqlConnection(_connectionString);

            var id = await connection.QuerySingleAsync<int>("AccountTypes_Insert",
                                                             new { userId = accountType.UserId,
                                                                    name = accountType.Name},
                                                             commandType: System.Data.CommandType.StoredProcedure);

            accountType.Id = id;

            
        }

        public async Task<bool> Exists(string name, int userId, int id = 0)
        {
            using var connection = new SqlConnection(_connectionString);
            var exists = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                            FROM AccountTypes
                                            WHERE Name = @Name AND UserId = @UserId
                                            AND Id <> @id;", new {name, userId, id});

            return exists == 1;
        }

        public async Task<IEnumerable<AccountType>> GetAll(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<AccountType>(@"SELECT Id, Name, [Order]
                                                                FROM AccountTypes
                                                                WHERE UserId = @UserId
                                                                ORDER BY [Order];", new {userId});

        }

        public async Task Update(AccountType accountType)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(@"UPDATE AccountTypes
                                            SET Name = @Name
                                            WHERE Id = @Id; ", accountType);
        }

        public async Task<AccountType> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<AccountType>(@"SELECT Id, Name, [Order]
                                                                            FROM AccountTypes
                                                                            WHERE Id = @Id AND UserId = @UserId;", new {id, userId});


        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync("DELETE AccountTypes WHERE Id = @Id;", new {id});
        }

        public async Task Order(IEnumerable<AccountType> accountTypesOrdered)
        {
            var query = "UPDATE AccountTypes SET [Order] = @Order Where Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, accountTypesOrdered);
        }
    }
}
