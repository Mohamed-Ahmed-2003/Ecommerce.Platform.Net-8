using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using OtlobLap.Configuration;
using OtlobLap.Models;
using OtlobLap.Services;
using OtlobLap.ViewModels;
using System.Diagnostics;

namespace OtlobLap.Controllers
{
    public class HomeController( IProductService productService,IProfileService profileService,ICategoryService categoryService, IBannerViewService bannerViewService) : BaseController(profileService)
    {
        private readonly ICategoryService _categoryService = categoryService;
        private readonly IProductService _productService = productService;
        private readonly IBannerViewService _bannerViewService = bannerViewService;

        public async Task<IActionResult> Index ()
        {
            var model = new HomeVM
            {
                Banners = (await _bannerViewService.GetCoverViewsAsync()).ToList(),
                Categories = await _categoryService.GetMostFilledCategoriesAsync(),
                PremiumProducts = await _productService.GetProductsWithTag(1,ProductsTag.Recommended),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> GetProducts( [Bind(Prefix = nameof(HomeVM.SettingsDto))] ProdViewSettingsDto settings)
        {

            var products = new List<Product>();

            products = await _productService.GetProductsWithOptionsAsync(settings);

            if (products == null || products.Count == 0)
                return BadRequest();

            if (settings.Mode == ViewMode.Horizontal)
                return PartialView("_PoductsLoadHorizontal", products);

            return PartialView("_PoductsLoadGrid",products);
        }
		[HttpPost]

		public async Task<IActionResult> GetPaginationControls (ProdViewSettingsDto settingsDto)
        {

            var model = new PaginationVM
            {
                TotalItems = await _productService.ProductsCount(settingsDto),
                CurrentPage = settingsDto.Page,
                PageSize = _productService.ProductsPageSize
            };


            return PartialView("_Pagination", model);
        }

	

        private IActionResult GetPartialViewForSortedProducts(IEnumerable<Product> sortedProducts, ViewMode mode)
		{
			if (mode == ViewMode.Horizontal)
				return PartialView("_PoductsLoadHorizontal", sortedProducts);
			else
				return PartialView("_PoductsLoadGrid", sortedProducts);
		}
      


		public async Task<IActionResult> ProductDetails(int Id)
        {
            var product = await _productService.GetProductByIdAsync(Id);
            ViewBag.SimilarProducts = await _productService.GetSimilarProductsAsync(product, 1);

            if (User.IsInRole("Customer") && ViewBag.UserId != null) 
                ViewBag.HasBought = await _productService.CustomerHasBrought(ViewBag.UserId, Id);
                    

			return View(product);
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index");
            }

            return LocalRedirect(returnUrl);
        }


        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Search(string input,int page = 1)
        {

            var products = await _productService.GetProductsBySearch(input, page);

			var productsResults = new ProductsResultsVM
			{
				Products = products,
				Brands = products.Where(p => p.Brand != null).Select(p => p.Brand).ToHashSet(),
				Categories = products.Where(p => p.ProductCategories != null).SelectMany(p => p.ProductCategories.Select(p => p.Category)).OrderBy(c => c.Name).ToHashSet(),
			};

			var VM = new SearchVM
            {
                Keyword = input,
                 Results = productsResults

            };

            return View(VM);
        }
        
        public async Task<IActionResult> SearchResults (string input,int page = 1)
        {
            var products = await _productService.GetProductsBySearch(input,page);

            if (products == null || products.Count == 0)
            {
                // Return a partial view result containing the message
                return BadRequest();
            }

            return PartialView("_PoductsLoadHorizontal", products);
        }
        public IActionResult AboutUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
