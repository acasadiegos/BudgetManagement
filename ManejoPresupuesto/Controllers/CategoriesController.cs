using ManejoPresupuesto.Interfaces;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriesController: Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserService _userService;

        public CategoriesController(ICategoryRepository categoryRepository,
            IUserService userService)
        {
            _categoryRepository = categoryRepository;
            _userService = userService;
        }

        public async Task<IActionResult> Index(PaginationViewModel paginationViewModel)
        {
            var userId = _userService.GetUserId();
            var categories = await _categoryRepository.GetAll(userId, paginationViewModel);
            var totalCategories = await _categoryRepository.Count(userId);

            var responseVM = new PaginationResponse<Category>
            {
                Elements = categories,
                Page = paginationViewModel.Page,
                RecordsPerPage = paginationViewModel.RecordsPerPage,
                TotalRecords = totalCategories,
                BaseURL = Url.Action() ?? string.Empty
            };

            return View(responseVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var userId = _userService.GetUserId();
            category.UserId = userId;
            await _categoryRepository.Create(category);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userService.GetUserId();
            var categorie = await _categoryRepository.GetById(id, userId);

            if(categorie is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(categorie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category categorieEdit)
        {
            if (!ModelState.IsValid)
            {
                return View(categorieEdit);
            }

            var userId = _userService.GetUserId();
            var categorie = await _categoryRepository.GetById(categorieEdit.Id, userId);

            if (categorie is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            categorieEdit.UserId = userId;

            await _categoryRepository.Update(categorieEdit);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userService.GetUserId();
            var categorie = await _categoryRepository.GetById(id, userId);

            if (categorie is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(categorie);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategorie(int id)
        {
            var userId = _userService.GetUserId();
            var categorie = await _categoryRepository.GetById(id, userId);

            if (categorie is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            await _categoryRepository.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
