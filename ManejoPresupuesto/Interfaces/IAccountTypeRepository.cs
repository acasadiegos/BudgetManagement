using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Interfaces
{
    public interface IAccountTypeRepository
    {
        Task Create(AccountType accountType);
        Task Delete(int id);
        Task<bool> Exists(string name, int userId, int id = 0);
        Task<IEnumerable<AccountType>> GetAll(int userId);
        Task<AccountType> GetById(int id, int userId);
        Task Order(IEnumerable<AccountType> accountTypesOrdered);
        Task Update(AccountType accountType);
    }
}
