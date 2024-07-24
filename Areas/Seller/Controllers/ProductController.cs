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


namespace OtlobLap.Areas.Seller.Controllers
{
    [Area(nameof(Utility.Roles.Seller))]
    [Authorize(Roles = nameof(Utility.Roles.Seller))]
    public class ProductController(IProductService productService,
                                ICategoryService categoryService,
                                UserManager<ApplicationUser> userManager,
                                IProfileService profileService,
                                IImageService imageService ,
                                ImageValidator imageValidator,
                                IBrandService brandService
        ) : BaseController(profileService)
    {
        private readonly IProductService _productService = productService;
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IBrandService _brandService = brandService;
        private readonly IImageService _imageService = imageService;
        private readonly ImageValidator _imageValidator = imageValidator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;


        [Authorize(Utility.Permissions.Product.View)]
        public async Task<IActionResult> Index()
        {
            int.TryParse(Request.Cookies["UserId"], out var userID);
            if (userID == 0) return RedirectToAction("Login", "Account");

            var products = await _productService.GetProductsBySellerIdAsync(userID);
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

        [Authorize(Utility.Permissions.Product.Add)]
        public async Task<IActionResult> Create()
        {
            ViewBag.view = Utility.GetTranslation("Add");

            ViewData["CategoryID"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "ID", "Name");
            ViewBag.Brands = await _brandService.GetAllBrandsAsync();
            return View();
        }

        [Authorize(Utility.Permissions.Product.Add)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM model)
        {
            ViewBag.view = Utility.GetTranslation("Add");

            model.Product.SellerID = (await _userManager.GetUserAsync(User)).Id;
            model.Product.AddedDate = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    // set image and seller 
                    #region Upload Product Image

                    if (model.MainProductImage != null)
                    {
                        model.ProductImages ??= new List<IFormFile>();
                        model.ProductImages.Add(model.MainProductImage);
                    }

                    if (model.ProductImages != null && _imageValidator.IsValidImageCollection(model.ProductImages.Count()))
                    {
                        model.Product.ProductImages = new List<ProductImage>();

                        foreach (var image in model.ProductImages)
                        {
                            if (_imageValidator.IsValidImage(image))
                            {
                                var productImage = new ProductImage
                                { URL = await _imageService.UploadPhysical(image, folderName: "Products") };
                                
                                model.Product.ProductImages.Add(productImage);

                                if (image.Equals(model.MainProductImage))
                                    model.Product.MainProductImage = productImage.URL;
                            }
                        }

                    }
                    #endregion
                    if (model.Categories != null)
                    {
                        model.Product.ProductCategories = new List<ProductCategory>();
                        foreach (var categoryId in model.Categories)
                        {
                            model.Product.ProductCategories.Add(new ProductCategory { CategoryId = categoryId});
                        }

                    }

                    await _productService.AddProductAsync(model.Product);

                    return RedirectToAction(nameof(Index));
                }
                ViewData["CategoryID"] = new SelectList(await _categoryService.GetAllCategoriesAsync(), "ID", "Name");
                ViewBag.Brands = await _brandService.GetAllBrandsAsync();

            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View(model);
        }

        [Authorize(Utility.Permissions.Product.Edit)]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.view = Utility.GetTranslation("Edit");

            var model = new ProductVM
            {
                Product = await _productService.GetProductByIdAsync(id)
            };

            if (model.Product == null)
            {
                return NotFound();
            }
           
            ViewBag.CategoryID = new SelectList(await _categoryService.GetAllCategoriesExcept(model.Product.ProductCategories.ToList()), "ID", "Name");
            ViewBag.Brands = await _brandService.GetAllBrandsAsync();

            return View(model);
        }

        [Authorize(Utility.Permissions.Product.Edit)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVM  model)
        {
            ViewBag.view = Utility.GetTranslation("Edit");

            if (id != model.Product.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                      var oldProduct = await _productService.GetProductByIdAsync(id);
                      
                    /////// old
                    int oldMainProduct = oldProduct.MainProductImage != null ? 1 : 0;
                    int oldSubImages = oldProduct.ProductImages != null ? oldProduct.ProductImages.Count - oldMainProduct : 0; 
                    /////// new
                    int mainImage= model.MainProductImage != null ? 1:oldMainProduct;
                    int subImages = model.ProductImages != null ? model.ProductImages.Count : oldSubImages;

                    if (_imageValidator.IsValidImageCollection(mainImage + subImages))
                    {
                        if (model.MainProductImage != null && _imageValidator.IsValidImage(model.MainProductImage)) // updating for main img 
                        {
                            var oldMainImg = oldProduct.ProductImages?.FirstOrDefault(img => img.URL == oldProduct.MainProductImage);
                            if (oldMainImg != null)
                            {
                                model.Product?.ProductImages?.Remove(oldMainImg); // remove from image list for product
                                await _productService.DeleteProductImage(oldMainImg);
                                _imageService.RemoveImagePhysical("Products", oldMainImg.URL);
                            }


                            var productImage = new ProductImage { URL = await _imageService.UploadPhysical(model.MainProductImage, folderName: "Products") };


                            model.Product.ProductImages ??= new List<ProductImage>();

                            model.Product.ProductImages.Add(productImage);
                            model.Product.MainProductImage = productImage.URL;
                        }
                        int MainImage = model.MainProductImage != null ? 1 : 0;

                        if (model.ProductImages != null)
                        {

                            foreach (var image in oldProduct.ProductImages)
                            {
                                if (image.URL == oldProduct.MainProductImage)
                                    oldProduct.ProductImages.Remove(image);
                                else 
                                _imageService.RemoveImagePhysical("Products", image.URL);  // remove old images physically
                            }

                            await _productService.DeleteProductImages(oldProduct); // remove locations from db
                           // i want to detach the oldProduct now

                            // add the new ones
                            model.Product.ProductImages ??= new List<ProductImage>();
                            foreach (var image in model.ProductImages)
                            {
                                if (_imageValidator.IsValidImage(image))
                                {
                                    var productImage = new ProductImage
                                    { URL = await _imageService.UploadPhysical(image, folderName: "Products") };

                                    model.Product.ProductImages.Add(productImage);


                                }
                            }


                        }
                        await _productService.RemoveProductCategories(model.Product.ID);
                            model.Product.ProductCategories = new List<ProductCategory>();
                        if (model.Categories != null)
                        {
                            foreach (var categoryId in model.Categories)
                            {
                                model.Product.ProductCategories.Add(new ProductCategory { CategoryId = categoryId });
                            }

                        }

                        await _productService.UpdateProductAsync(model.Product);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.error = ex.Message;
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryID = new SelectList(await _categoryService.GetAllCategoriesExcept(model.Product.ProductCategories.ToList()), "ID", "Name");
            ViewBag.Brands = await _brandService.GetAllBrandsAsync();


            return View(model);
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
        [Authorize(Utility.Permissions.Product.Edit)]

        public async Task<IActionResult> RemoveImage(int productId, string  imageUrl)
        {
            // Retrieve the product based on productId
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
            {
                // Handle the case where the product with the given productId is not found
                return NotFound();
            }

            // Find the image with the specified imageId
            var imageToRemove = product.ProductImages?.FirstOrDefault(img => img.URL == imageUrl);

            if (imageToRemove == null)
            {
                // Handle the case where the image with the given imageId is not found
                return NotFound();
            }
            product.ProductImages?.Remove(imageToRemove);

            if (imageToRemove.URL == product.MainProductImage) 
                product.MainProductImage = null;

            // Delete the physical image file
            _imageService.RemoveImagePhysical("Products", imageToRemove.URL);

            // Remove the image from the product
            await _productService.DeleteProductImage(imageToRemove);

            await _productService.UpdateProductAsync(product);

            // Redirect back to the Edit view or any other appropriate action
            return RedirectToAction(nameof(Edit), new { id = productId });
        }


        private bool ProductExists(int id)
        {
            return _productService.GetProductByIdAsync(id) != null;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.Controller = Utility.GetTranslation("Products");
        }

    }
}
