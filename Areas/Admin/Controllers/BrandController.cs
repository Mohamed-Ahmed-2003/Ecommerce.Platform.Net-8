using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Services;
using OtlobLap.ViewModels;

namespace OtlobLap.Areas.Admin.Controllers
{
    [Area(nameof(Utility.Roles.Admin))]
    [Authorize(Roles = nameof(Utility.Roles.Admin))]
    public class BrandController(IProfileService profileService , IBrandService brandService) : BaseController(profileService)
    {
        private readonly IBrandService _brandService = brandService;


        public async Task<IActionResult> Index()
        {
            var model = new ManageBrandsVM
            {
                Brands = await _brandService.GetAllBrandsAsync()
            };

            return View(model);
        }


        public async Task<IActionResult> Details(int id)
        {


            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            ViewBag.view = Utility.GetTranslation("Details"); ;
            return View(brand);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManageBrandsVM VM)
        {

            await _brandService.AddBrandAsync(VM.BrandVM);
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ManageBrandsVM VM)
        {


            try
            {
                await _brandService.UpdateBrandAsync(VM.BrandVM);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();

            }
            return RedirectToAction(nameof(Index));

        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            var brand = await _brandService.GetBrandByIdAsync(Id);
            if (brand != null)
            {
                await _brandService.DeleteBrandAsync(Id);
            }

            return RedirectToAction(nameof(Index));
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.Controller = Utility.GetTranslation("Brands");
        }

    }
}
