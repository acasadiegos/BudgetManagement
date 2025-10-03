using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Interfaces
{
    public interface IReportService
    {
        Task<DetailTransactionReport> GetDetailTransactionByAccountReport(int userId, int accountId, int month, int year, dynamic ViewBag);
        Task<DetailTransactionReport> GetDetailTransactionReport(int userId, int month, int year, dynamic ViewBag);
        Task<IEnumerable<GetByWeekResult>> GetWeeklyReport(int userId, int month, int year, dynamic ViewBag);
    }
}
