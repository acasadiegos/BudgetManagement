using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Interfaces
{
    public interface ITransactionsRepository
    {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<IEnumerable<Transaction>> GetByAccountId(GetTransactionsByAccount model);
        Task<Transaction> GetById(int id, int userId);
        Task<IEnumerable<GetByMonthResult>> GetByMonth(int userId, int year);
        Task<IEnumerable<Transaction>> GetByUserId(GetTransactionsByUserParams model);
        Task<IEnumerable<GetByWeekResult>> GetByWeek(GetTransactionsByUserParams model);
        Task Update(Transaction transaction, decimal previousAmount, int previousAccount);
    }
}
