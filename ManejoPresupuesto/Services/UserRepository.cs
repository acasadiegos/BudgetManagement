using Dapper;
using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateUser(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            var userId = await connection.QuerySingleAsync<int>(@"INSERT INTO Users (Email, NormalizedEmail, PasswordHash)
                                                            VALUES (@Email, @NormalizedEmail, @PasswordHash);
                                                            SELECT SCOPE_IDENTITY();", user);

            await connection.ExecuteAsync("CreateDataNewUser", new { userId },
                commandType: System.Data.CommandType.StoredProcedure);

            return userId;
        }

        public async Task<User> GetUserByEmail(string normalizedEmail)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(@"SELECT * 
                                                                        FROM Users 
                                                                        WHERE NormalizedEmail = @normalizedEmail",
                                                                        new {normalizedEmail});
        }
    }
}
