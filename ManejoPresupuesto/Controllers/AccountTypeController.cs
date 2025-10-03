using Dapper;
using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class AccountTypeController : Controller
    {
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly IUserService _userService;

        public AccountTypeController(IAccountTypeRepository accountTypeRepository,
            IUserService userService)
        {
            _accountTypeRepository = accountTypeRepository;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userService.GetUserId();
            var accountTypes = await _accountTypeRepository.GetAll(userId);
            return View(accountTypes);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountType accountType)
        {
            if(!ModelState.IsValid)
            {
                return View(accountType);
            }

            accountType.UserId = _userService.GetUserId();

            var alreadyExists = await _accountTypeRepository.Exists(accountType.Name, accountType.UserId);

            if(alreadyExists)
            {
                ModelState.AddModelError(nameof(accountType.Name), $"The name {accountType.Name} already exists.");

                return View(accountType);
            }

            await _accountTypeRepository.Create(accountType);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var userId = _userService.GetUserId();
            var accountType = await _accountTypeRepository.GetById(id, userId);

            if(accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AccountType accountType)
        {
            var userId = _userService.GetUserId();
            var accountTypeExists = await _accountTypeRepository.GetById(accountType.Id, userId);

            if(accountTypeExists is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountTypeRepository.Update(accountType);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userService.GetUserId();

            var accountType = await _accountTypeRepository.GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(accountType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountType(int id)
        {
            var userId = _userService.GetUserId();

            var accountType = await _accountTypeRepository.GetById(id, userId);

            if (accountType is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _accountTypeRepository.Delete(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyExistsAccountType(string name, int id)
        {
            var userId = _userService.GetUserId();

            var alreadyExitsAccountType = await _accountTypeRepository.Exists(name, userId, id);

            if (alreadyExitsAccountType)
            {
                return Json($"The name {name} already exits");
            }

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Order([FromBody] int[] ids)
        {
            var userId = _userService.GetUserId();
            var accountTypes = await _accountTypeRepository.GetAll(userId);
            var idsAccountTypes = accountTypes.Select(x => x.Id);

            var idsAccountTypesNotBelongToUser = ids.Except(idsAccountTypes).ToList();

            if(idsAccountTypesNotBelongToUser.Count > 0)
            {
                return Forbid();
            }

            var accountTypesOrdered = ids.Select((value, index) =>
                                        new AccountType() { Id = value, Order = index + 1 }).AsEnumerable();

            await _accountTypeRepository.Order(accountTypesOrdered);

            return Ok();
        }
    }
}
