using AutoMapper;
using ClosedXML.Excel;
using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace ManejoPresupuesto.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IReportService _reportService;

        public TransactionsController(IUserService userService, ITransactionsRepository transactionsRepository,
            IAccountRepository accountRepository, ICategoryRepository categoryRepository,
            IMapper mapper, IReportService reportService)
        {
            this._userService = userService;
            this._transactionsRepository = transactionsRepository;
            this._accountRepository = accountRepository;
            this._categoryRepository = categoryRepository;
            this._mapper = mapper;
            this._reportService = reportService;
        }

        public async Task<IActionResult> Weekly(int month, int year)
        {
            var userId = _userService.GetUserId();
            IEnumerable<GetByWeekResult> weeklyTransactions = 
                await _reportService.GetWeeklyReport(userId, month, year, ViewBag);

            var group = weeklyTransactions.GroupBy(x => x.Week).Select(x =>
                new GetByWeekResult()
                {
                    Week = x.Key,
                    Incomes = x.Where(x => x.OperationTypeId == OperationType.Income).Select(x => x.Amount).FirstOrDefault(),
                    Egress = x.Where(x => x.OperationTypeId == OperationType.Expense).Select(x => x.Amount).FirstOrDefault()
                }).ToList();

            if(year == 0 || month == 0)
            {
                var today = DateTime.Today;
                year = today.Year;
                month = today.Month;
            }

            var referenceDate = new DateTime(year, month, 1);
            var monthDays = Enumerable.Range(1, referenceDate.AddMonths(1).AddDays(-1).Day);

            var segmentedDays = monthDays.Chunk(7).ToList();

            for(int i=0; i<segmentedDays.Count; i++)
            {
                var week = i + 1;
                var startDate = new DateTime(year, month, segmentedDays[i].First());
                var endDate = new DateTime(year, month, segmentedDays[i].Last());
                var weekGroup = group.FirstOrDefault(x => x.Week == week);

                if(weekGroup is null)
                {
                    group.Add(new GetByWeekResult
                    {
                        Week = week,
                        StartDate = startDate,
                        EndDate = endDate

                    });
                }
                else
                {
                    weekGroup.StartDate = startDate;
                    weekGroup.EndDate = endDate;
                }
            }

            group = group.OrderByDescending(x => x.Week).ToList();

            var model = new WeeklyReportViewModel();
            model.WeeklyTransactions = group;
            model.DateReference = referenceDate;

            return View(model);
        }

        public async Task<IActionResult> Monthly(int year)
        {
            var userId = _userService.GetUserId();

            if (year == 0)
            {
                year = DateTime.Today.Year;
            }

            var monthlyTransactions = await _transactionsRepository.GetByMonth(userId, year);

            var groupTransactions = monthlyTransactions.GroupBy(x => x.Month)
                .Select(x => new GetByMonthResult()
                {
                    Month = x.Key,
                    Income = x.Where(x => x.OperationTypeId == OperationType.Income)
                        .Select(x => x.Amount).FirstOrDefault(),
                    Egress = x.Where(x => x.OperationTypeId == OperationType.Expense)
                        .Select(x => x.Amount).FirstOrDefault()
                }).ToList();

            for (int month = 1; month <= 12; month++)
            {
                var transaction = groupTransactions.FirstOrDefault(x => x.Month == month);
                var dateReference = new DateTime(year, month, 1);

                if (transaction is null)
                {
                    groupTransactions.Add(new GetByMonthResult()
                    {
                        Month = month,
                        ReferenceDate = dateReference,
                    });
                }
                else
                {
                    transaction.ReferenceDate = dateReference;
                }
            }

            groupTransactions = groupTransactions.OrderByDescending(x => x.Month).ToList();

            var model = new MonthlyReportViewModel();
            model.Year = year;
            model.MonthlyTransactions = groupTransactions;

            return View(model);
        }

        public IActionResult ExcelReport()
        {
            return View();
        }

        [HttpGet]
        public async Task<FileResult> ExportExcelByMonth(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var userId = _userService.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(
                new GetTransactionsByUserParams
                {
                    UserId = userId,
                    StartDate = startDate,
                    EndDate = endDate
                });

            var fileName = $"Budget Management - {startDate.ToString("MMM yyyy")}.xlsx";

            return GenerateExcel(fileName, transactions);
        }

        [HttpGet]
        public async Task<FileResult> ExportExcelByYear(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1).AddDays(-1);
            var userId = _userService.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(
                                    new GetTransactionsByUserParams
                                    {
                                        UserId = userId,
                                        StartDate = startDate,
                                        EndDate = endDate
                                    });

            var fileName = $"Budget Management - {startDate.ToString("yyyy")}.xlsx";
            return GenerateExcel(fileName, transactions);

        }

        [HttpGet]
        public async Task<FileResult> ExportExcelAll()
        {
            var startDate = DateTime.Today.AddYears(-100);
            var endDate = DateTime.Today.AddYears(1000);
            var userId = _userService.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(
                        new GetTransactionsByUserParams
                        {
                            UserId = userId,
                            StartDate = startDate,
                            EndDate = endDate
                        });

            var fileName = $"Budget Management - {DateTime.Today.ToString("dd-MM-yyyy")}.xlsx";

            return GenerateExcel(fileName, transactions);
        }

        private FileResult GenerateExcel(string fileName, IEnumerable<Transaction> transactions)
        {
            DataTable dataTable = new DataTable("Transactions");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Date"),
                new DataColumn("Account"),
                new DataColumn("Categorie"),
                new DataColumn("Note"),
                new DataColumn("Amount"),
                new DataColumn("Income/Egress"),
            });

            foreach(var transaction in transactions)
            {
                dataTable.Rows.Add(transaction.TransactionDate,
                    transaction.Account,
                    transaction.Categorie,
                    transaction.Note,
                    transaction.Amount,
                    transaction.OperationTypeId);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                        fileName);
                }
            }
        }


        public IActionResult Calendar()
        {
            return View();
        }

        public async Task<JsonResult> GetCalendarTransactions(DateTime start, DateTime end)
        {
            var userId = _userService.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(
                new GetTransactionsByUserParams
                {
                    UserId = userId,
                    StartDate = start,
                    EndDate = end
                });

            var calendarEvents = transactions.Select(transaction => new CalendarEvent()
            {
                Title = transaction.Amount.ToString("N"),
                Start = transaction.TransactionDate.ToString("yyyy-MM-dd"),
                End = transaction.TransactionDate.ToString("yyyy-MM-dd"),
                Color = (transaction.OperationTypeId == OperationType.Expense) ? "Red" : null
            });

            return Json(calendarEvents);
        }

        public async Task<JsonResult> GetTransactionsByDate(DateTime date)
        {
            var userId = _userService.GetUserId();

            var transactions = await _transactionsRepository.GetByUserId(
                                new GetTransactionsByUserParams
                                {
                                    UserId = userId,
                                    StartDate = date,
                                    EndDate = date
                                });

            return Json(transactions);
        }

        public async Task<IActionResult> Index(int month, int year)
        {
            var userId = _userService.GetUserId();

            var model = await _reportService.GetDetailTransactionReport(userId, month, year, ViewBag);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var userId = _userService.GetUserId();
            var model = new CreateTransactionViewModel();
            model.Accounts = await GetUserAccounts(userId);
            model.Categories = await GetCategories(userId, model.OperationTypeId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionViewModel model)
        {
            var userId = _userService.GetUserId();

            if (!ModelState.IsValid)
            {
                model.Accounts = await GetUserAccounts(userId);
                model.Categories = await GetCategories(userId, model.OperationTypeId);
                return View(model);
            }

            var account = await _accountRepository.GetById(model.AccountId, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var categorie = await _categoryRepository.GetById(model.CategorieId, userId);

            if(categorie is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            model.UserId = userId;

            if(model.OperationTypeId == OperationType.Expense)
            {
                model.Amount *= -1;
            }

            await _transactionsRepository.Create(model);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string urlRetorno = "")
        {
            var userId = _userService.GetUserId();
            var transaction = await _transactionsRepository.GetById(id, userId);

            if (transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = _mapper.Map<UpdateTransactionViewModel>(transaction);

            model.PreviousAmount = model.Amount;

            if (model.OperationTypeId == OperationType.Expense)
            {
                model.PreviousAmount = model.Amount * -1;
            }

            model.PreviousAccountId = transaction.AccountId;
            model.Categories = await GetCategories(userId, transaction.OperationTypeId);
            model.Accounts = await GetUserAccounts(userId);
            model.UrlRetorno = urlRetorno;

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateTransactionViewModel model)
        {
            var userId = _userService.GetUserId();

            if(!ModelState.IsValid)
            {
                model.Accounts = await GetUserAccounts(userId);
                model.Categories = await GetCategories(userId, model.OperationTypeId);
                return View(model);
            }

            var account = await _accountRepository.GetById(model.AccountId, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var categorie = await _categoryRepository.GetById(model.CategorieId, userId);

            if(categorie is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var transaction = _mapper.Map<Transaction>(model);

            if(model.OperationTypeId == OperationType.Expense)
            {
                transaction.Amount *= -1;
            }

            await _transactionsRepository.Update(transaction, model.PreviousAmount, 
                model.PreviousAccountId);

            if(string.IsNullOrEmpty(model.UrlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(model.UrlRetorno);
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string urlRetorno = "")
        {
            var userId = _userService.GetUserId();

            var transaction = await _transactionsRepository.GetById(id, userId);

            if(transaction is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _transactionsRepository.Delete(id);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
        }

        private async Task<IEnumerable<SelectListItem>> GetUserAccounts(int userId)
        {
            var accounts = await _accountRepository.Search(userId);
            return accounts.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> GetCategories(int userId, OperationType operationType)
        {
            var categories = await _categoryRepository.GetAll(userId, operationType);
            var result = categories.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();

            var defaultOption = new SelectListItem("-- Select a Category --", "0", true);

            result.Insert(0, defaultOption);

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> GetCategoriesByOperationType([FromBody] OperationType operationType)
        {
            var userId = _userService.GetUserId();
            var categories = await GetCategories(userId, operationType);
            return Ok(categories);
        }

    }
}
