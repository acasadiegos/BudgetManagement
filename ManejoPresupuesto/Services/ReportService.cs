using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services
{
    public class ReportService: IReportService
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly HttpContext _httpContext;

        public ReportService(ITransactionsRepository transactionsRepository,
                IHttpContextAccessor httpContextAccessor)
        {
            _transactionsRepository = transactionsRepository;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<GetByWeekResult>> GetWeeklyReport(int userId, int month, int year, dynamic ViewBag)
        {
            (DateTime startDate, DateTime endDate) = GenerateStartAndEndDate(month, year);

            var parameters = new GetTransactionsByUserParams()
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            SetViewBagValues(ViewBag, startDate);

            var model = await _transactionsRepository.GetByWeek(parameters);

            return model;
        }

        public async Task<DetailTransactionReport> GetDetailTransactionReport(int userId, int month, int year, dynamic ViewBag)
        {
            (DateTime startDate, DateTime endDate) = GenerateStartAndEndDate(month, year);

            var parameters = new GetTransactionsByUserParams()
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            var transactions = await _transactionsRepository.GetByUserId(parameters);

            var model = GenerateDetailTransactionReport(startDate, endDate, transactions);

            SetViewBagValues(ViewBag, startDate);

            return model;
        }
        public async Task<DetailTransactionReport> GetDetailTransactionByAccountReport(int userId, int accountId,
            int month, int year, dynamic ViewBag)
        {
            (DateTime startDate, DateTime endDate) = GenerateStartAndEndDate(month, year);

            var getTransactionsByAccount = new GetTransactionsByAccount()
            {
                AccountId = accountId,
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            var transactions = await _transactionsRepository
                                        .GetByAccountId(getTransactionsByAccount);

            var model = GenerateDetailTransactionReport(startDate, endDate, transactions);

            SetViewBagValues(ViewBag, startDate);

            return model;
        }

        private void SetViewBagValues(dynamic ViewBag, DateTime startDate)
        {
            ViewBag.previousMonth = startDate.AddMonths(-1).Month;
            ViewBag.previousYear = startDate.AddMonths(-1).Year;

            ViewBag.nextMonth = startDate.AddMonths(1).Month;
            ViewBag.nextYear = startDate.AddMonths(1).Year;

            ViewBag.urlRetorno = _httpContext.Request.Path + _httpContext.Request.QueryString;
        }

        private static DetailTransactionReport GenerateDetailTransactionReport(DateTime startDate, DateTime endDate, IEnumerable<Transaction> transactions)
        {
            var model = new DetailTransactionReport();

            var transactionsByDate = transactions.OrderByDescending(x => x.TransactionDate)
                .GroupBy(x => x.TransactionDate)
                .Select(group => new DetailTransactionReport.TransactionsByDate()
                {
                    TransactionDate = group.Key,
                    Transactions = group.AsEnumerable()
                });

            model.GroupTransactions = transactionsByDate;
            model.StartDate = startDate;
            model.EndDate = endDate;
            return model;
        }

        private (DateTime startDate, DateTime endDate) GenerateStartAndEndDate(int month, int year)
        {
            DateTime startDate;
            DateTime endDate;

            if (month <= 0 || month > 12 || year <= 1900)
            {
                var today = DateTime.Today;
                startDate = new DateTime(today.Year, today.Month, 1);
            }
            else
            {
                startDate = new DateTime(year, month, 1);
            }

            endDate = startDate.AddMonths(1).AddDays(-1);

            return (startDate, endDate);
        }
    }
}
