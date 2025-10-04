using Dapper;
using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;
        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Category category)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categories (Name, OperationTypeId, UserId)
                                                               VALUES (@Name, @OperationTypeId, @UserId);
                                                               SELECT SCOPE_IDENTITY();", category);

            category.Id = id;
        }

        public async Task<int> Count(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM Categories WHERE UserId = @userId", new {userId}
                );
        }

        public async Task<IEnumerable<Category>> GetAll(int userId, PaginationViewModel pagination)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Category>(
                        @$"SELECT * 
                         FROM Categories
                        WHERE UserId = @UserId
                        ORDER BY Name
                        OFFSET {pagination.RecordsToSkip} ROWS FETCH NEXT {pagination.RecordsPerPage} 
                        ROWS ONLY", 
                         new { userId });

        }
        public async Task<IEnumerable<Category>> GetAll(int userId, OperationType operationTypeId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Category>(@"SELECT * FROM Categories 
                                                            WHERE UserId = @UserId 
                                                            AND OperationTypeId = @operationTypeId",
                                                            new { userId, operationTypeId });

        }

        public async Task<Category> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>(@"SELECT * FROM Categories WHERE Id = @Id AND UserId = @UserId",
                                                                           new {id,userId});

        }

        public async Task Update(Category category)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"UPDATE Categories 
                                            SET Name = @Name, OperationTypeId = @OperationTypeId
                                            WHERE Id = @Id;",
                                            category);
        }


        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"DELETE Categories WHERE Id = @Id", new {id});
        }
    }
}
