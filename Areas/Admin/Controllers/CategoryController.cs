using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.Services;
using OtlobLap.ViewModels;

namespace OtlobLap.Areas.Admin.Controllers
{
    [Area(nameof(Utility.Roles.Admin))]
    [Authorize(Roles = nameof(Utility.Roles.Admin))]
    public class CategoryController(ICategoryService categoryService, IProfileService profileService) : BaseController(profileService)
    {
        private readonly ICategoryService _categoryService = categoryService;

        [Authorize(Utility.Permissions.Category.View)]

        public async Task<IActionResult> Index()
        {
            var model = new ManageCategoriesVM
            {
                Categories = await _categoryService.GetAllCategoriesAsync()
            };

            return View(model);
        }

        [Authorize(Utility.Permissions.Category.View)]

        public async Task<IActionResult> Details(int id)
        {


            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            ViewBag.view = Utility.GetTranslation("Details"); ;
            return View(category);
        }


        [Authorize(Utility.Permissions.Category.Add)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManageCategoriesVM VM)
        {
            ViewBag.view = Utility.GetTranslation("Add");

                await _categoryService.AddCategoryAsync(VM.CategoryVM);
               return RedirectToAction(nameof(Index));
            
            return View(VM);
        }



        [Authorize(Utility.Permissions.Category.Edit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ManageCategoriesVM VM)
        {

         
                try
                {
                    await _categoryService.UpdateCategoryAsync(VM.CategoryVM);
                }
                catch (DbUpdateConcurrencyException)
                {
                        return NotFound();
                  
                }
                return RedirectToAction(nameof(Index));
            
        }

   

        [Authorize(Utility.Permissions.Category.Remove)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(Id);
            if (category != null)
            {
                await _categoryService.DeleteCategoryAsync(Id);
            }

            return RedirectToAction(nameof(Index));
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.Controller = Utility.GetTranslation("CategoriesTitle");
        }

    }
}
