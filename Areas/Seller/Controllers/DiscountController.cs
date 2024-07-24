
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.Services;

namespace OtlobLap.Areas.Seller.Controllers
{
    [Area(nameof(Utility.Roles.Seller))]
    [Authorize(Roles = nameof(Utility.Roles.Seller))]
    public class DiscountController(IDiscountService discountService , IProductService productService , IProfileService profileService) : BaseController(profileService)
    {
        private readonly IDiscountService _discountService = discountService;
        private readonly IProductService _productService  = productService;

        [Authorize(Utility.Permissions.Discount.Edit)]

        public async Task ToggleDiscountStatus(int id)
        {
            await _discountService.ToggleDiscountStatus(id);
        }
        [Authorize(Utility.Permissions.Discount.View)]
        public async Task<IActionResult>Index()
        {
            ViewBag.Controller = Utility.GetTranslation("Discounts");

            int.TryParse(Request.Cookies["UserId"], out var userID);
            if (userID == 0) return RedirectToAction("Login","Account");

            var discounts = await _discountService.GetAllDiscounts();
            var products  = await _productService.GetProductsBySellerIdAsync(userID);
            ViewBag.productsList = new SelectList(products,"ID","Name");
            return View(discounts);
        }
        [Authorize(Utility.Permissions.Discount.Add)]

        [HttpPost]
        public async Task<IActionResult> AddDiscount(Discount discount)
        {

            if (discount == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _discountService.AddDiscount(discount);
                return Ok(discount);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.Error.WriteLine("Exception occurred: " + ex.Message);
                return BadRequest();
            }
        }
        [Authorize(Utility.Permissions.Discount.Edit)]

        [HttpPost]
        public async Task<IActionResult> EditDiscount(Discount modifiedDiscount)
        {

            var discount = await _discountService.GetDiscountAsync(modifiedDiscount.Id);

            if (discount == null)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.TryGetValue("Percentage", out var PercentageState) && PercentageState.Errors.Any())
                    throw new Exception();
                else 
                discount.Percentage = modifiedDiscount.Percentage;

                if (modifiedDiscount.EndDate != default(DateTime) 
                    //&& 
                    //ModelState.TryGetValue("EndDate", out var EndDateState) && EndDateState.Errors.Any()
                    ) 
                discount.EndDate = modifiedDiscount.EndDate;
                await _discountService.UpdateDiscount(discount);
                return Ok(discount);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.Error.WriteLine("Exception occurred: " + ex.Message);
                return BadRequest();
            }
        }
        [Authorize(Utility.Permissions.Discount.Remove)]

        [HttpPost]
        public async Task<IActionResult> RemoveDiscount(int id)
        {

            try
            {
                await _discountService.DeleteDiscount(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.Error.WriteLine("Exception occurred: " + ex.Message);
                return BadRequest();
            }
        }

       
    }
}
