using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Services;
using OtlobLap.ViewModels;

namespace OtlobLap.Areas.Seller.Controllers
{
    [Area(nameof(Utility.Roles.Seller))]
    [Authorize(Roles = nameof(Utility.Roles.Seller))]
    public class HomeController(IProfileService profileService , ISellerHomeService sellerHomeService) : BaseController(profileService)
    {
        private readonly ISellerHomeService _sellerHomeService = sellerHomeService;
        public async Task<IActionResult> Index()
        {
            var model = await _sellerHomeService.GetSellerInfoAsync(ViewBag.UserId);
            ViewBag.Controller = Utility.GetTranslation("CategoriesTitle");
            ViewBag.view= Utility.GetTranslation("Data");

            return View(model);
        }
        public async Task<IActionResult> GetSalesCount(FilterOption filterOption)
        {
            // Call the service method to get sales count based on filter option
            var salesCount = await _sellerHomeService.GetSalesCountAsync(ViewBag.UserId, filterOption);

            return Json(new {sales = salesCount.Item1, percentIncrease = salesCount.Item2 == 0
                        ? (salesCount.Item1 > 0 ? 100 : 0)  // If previous sales is zero, check if current sales is greater than zero, then set to 100, otherwise set to 0
                        : ((salesCount.Item1 - salesCount.Item2) * 100 / salesCount.Item2)});
        }

        public async Task<IActionResult> GetRevenue(FilterOption filterOption)
        {
            var RevenueInfo = await _sellerHomeService.GetRevenueAsync(ViewBag.UserId, filterOption);

            string currRevenue = RevenueInfo.Item1.ToString("C");

            return Json(new
            {
                Revenue = currRevenue,
                percentIncrease = RevenueInfo.Item2 == 0
                        ? (RevenueInfo.Item1 > 0 ? 100 : 0) 
                        : ((RevenueInfo.Item1 - RevenueInfo.Item2) * 100 / RevenueInfo.Item2)
            });
        }


    }
}
