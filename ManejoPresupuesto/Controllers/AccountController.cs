using AutoMapper;
using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly IUserService _userService;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IReportService _reportService;

        public AccountController(IAccountTypeRepository accountTypeRepository,
            IUserService userService, IAccountRepository accountRepository,
            IMapper mapper,
            ITransactionsRepository transactionsRepository,
            IReportService reportService)
        {
            _accountTypeRepository = accountTypeRepository;
            _userService = userService;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _transactionsRepository = transactionsRepository;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userService.GetUserId();
            var accountWithAccountType = await _accountRepository.Search(userId);

            var model = accountWithAccountType
                    .GroupBy(x => x.AccountType)
                    .Select(group => new IndexAccountViewModel
                    {
                        AccountType = group.Key,
                        Accounts = group.AsEnumerable()
                    }).ToList();

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _userService.GetUserId();

            var model = new AccountCreationViewModel();
            model.AccountTypes = await GetAccountTypes(userId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountCreationViewModel account)
        {
            var userId = _userService.GetUserId();
            var accountType = _accountTypeRepository.GetById(account.AccountTypeId, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if(!ModelState.IsValid)
            {
                account.AccountTypes = await GetAccountTypes(userId);
                return View(account);
            }

            await _accountRepository.Create(account);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userService.GetUserId();
            var account = await _accountRepository.GetById(id, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var model = _mapper.Map<AccountCreationViewModel>(account);

            model.AccountTypes = await GetAccountTypes(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountCreationViewModel accountEdit)
        {
            var userId = _userService.GetUserId();
            var account = await _accountRepository.GetById(accountEdit.Id, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var accountType = await _accountTypeRepository.GetById(accountEdit.AccountTypeId, userId);

            if(accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountRepository.Update(accountEdit);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userService.GetUserId();
            var account = await _accountRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var userId = _userService.GetUserId();
            var account = await _accountRepository.GetById(id, userId);

            if (account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountRepository.Delete(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id, int month, int year)
        {
            var userId = _userService.GetUserId();
            var account = await _accountRepository.GetById(id, userId);

            if(account is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            ViewBag.Account = account.Name;

            var model = await _reportService.GetDetailTransactionByAccountReport(userId, id, month, year, ViewBag);

            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetAccountTypes(int userId)
        {
            var accountTypes = await _accountTypeRepository.GetAll(userId);
            return accountTypes.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
        }
    }
}
