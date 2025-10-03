using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Interfaces
{
    public interface IAccountRepository
    {
        Task Create(Account account);
        Task Delete(int id);
        Task<Account> GetById(int id, int userId);
        Task<IEnumerable<Account>> Search(int userId);
        Task Update(AccountCreationViewModel account);
    }
}
