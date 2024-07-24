using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.Services;
using OtlobLap.Validations;
using OtlobLap.ViewModels;


namespace OtlobLap.Areas.Admin.Controllers
{
    [Area(nameof(Utility.Roles.Admin))]
    [Authorize(Roles = nameof(Utility.Roles.Admin))]
    public class ProductController(IProductService productService,
                                ICategoryService categoryService,
                                UserManager<ApplicationUser> userManager,
                                IProfileService profileService,
                                IImageService imageService ,
                                ImageValidator imageValidator
        ) : BaseController(profileService)
    {
        private readonly IProductService _productService = productService;
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IImageService _imageService = imageService;
        private readonly ImageValidator _imageValidator = imageValidator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;


        [Authorize(Utility.Permissions.Product.View)]
        public async Task<IActionResult> Index()
        {

            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [Authorize(Utility.Permissions.Product.View)]
        public async Task<IActionResult> Details(int id)
        {
            ViewBag.view = Utility.GetTranslation("Details");
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> TogglePremium(int Id)
        {
            await _productService.TogglePremiumProduct(Id);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> ToggleHide (int Id)
        {
            await _productService.ToggleHideProduct(Id);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Utility.Permissions.Product.Remove)]
        public async Task<IActionResult> Delete(int id)
        {

            ViewBag.view = Utility.GetTranslation("Delete");

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [Authorize(Utility.Permissions.Product.Remove)]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product != null)
            {
                if (product.ProductImages != null)
                {
                    foreach (var image in product.ProductImages) {
                        _imageService.RemoveImagePhysical("Products",image.URL);
                    }
                }
                await _productService.DeleteProductImages(product);
                await _productService.DeleteProductAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }
 

    }
}
