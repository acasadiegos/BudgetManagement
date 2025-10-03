using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Interfaces
{
    public interface ICategoryRepository
    {
        Task<int> Count(int userId);
        Task Create(Category category);
        Task Delete(int id);
        Task<IEnumerable<Category>> GetAll(int userId, PaginationViewModel pagination);
        Task<IEnumerable<Category>> GetAll(int userId, OperationType operationTypeId);
        Task<Category> GetById(int id, int userId);
        Task Update(Category category);
    }
}
