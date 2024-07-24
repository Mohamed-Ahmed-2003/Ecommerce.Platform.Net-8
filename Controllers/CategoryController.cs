using Microsoft.AspNetCore.Mvc;
using OtlobLap.Services;

namespace OtlobLap.Controllers
{
    public class CategoryController(IProfileService profileService , ICategoryService categoryService) : BaseController(profileService)
    {
        private readonly ICategoryService _categoryService = categoryService;
        public async Task<IActionResult> Index(int Id)
        {
            var model = await _categoryService.GetCategoryProductsAsync(Id,1);

            return View(model);
        }
    }
}
