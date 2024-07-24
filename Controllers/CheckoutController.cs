using Microsoft.AspNetCore.Mvc;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.Services;
using Stripe.Checkout;


namespace OtlobLap.Controllers
{
    public class CheckoutController(IProfileService profileService , ICartService cartService , IOrderService orderService) : BaseController(profileService)
    {
        private readonly ICartService _cartService = cartService;
        private readonly IOrderService _orderService = orderService;

        public async Task<IActionResult> OrderConfirmation()
        {
            var service = new SessionService();
            var session = service.Get(TempData["SessionId"].ToString());

            if (session.PaymentStatus.Equals("Paid",StringComparison.OrdinalIgnoreCase))
            {
                var cart = await _cartService.GetCartAsync(ViewBag.UserId) as Cart;

                // Apply any discounts if found
                foreach (var orderItem in cart.OrderItems)
                {
                    orderItem.Price = Utility.CalculateDiscountedPrice(orderItem.Price, orderItem.Product?.Discount?.Percentage ?? 0);
                }

                var OrderItemsBySeller = cart.OrderItems.GroupBy(item => item.Product?.SellerID);


                foreach (var items in OrderItemsBySeller)
                {
                    var OrderItems = items.Select(OrderItem => OrderItem).ToList();


                    var order = new Order
                    {
                        CustomerId = ViewBag.UserId,
                        SellerId = (int)OrderItems[0].Product.SellerID,
                        Date = DateTime.Now,
                        ArrivalDate = default,
                        OrderItems = OrderItems,

                    };
                    await _orderService.CreateOrderAsync(order);
                }

                await _cartService.ClearCartAsync(ViewBag.UserId);
                return RedirectToAction("Index","Order");
            }


            return RedirectToAction("CheckoutFailure");
        }
        public IActionResult CheckoutFailure()
        {
            return View();
        }
		public async Task<IActionResult> CheckoutAsync()
		{
			var cart = await _cartService.GetCartAsync(ViewBag.UserId) as Cart;

			// Get the current request's scheme and host
			var domain = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

			if (cart != null && cart.OrderItems != null && cart.OrderItems.Any())
			{
				var options = new SessionCreateOptions
				{
					SuccessUrl = $"{domain}{Url.Action("OrderConfirmation", "Checkout")}",
					CancelUrl = $"{domain}{Url.Action("CheckoutFailure", "Checkout")}",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
					CustomerEmail = cart.Customer?.Email,
					ShippingAddressCollection = new SessionShippingAddressCollectionOptions
					{
						AllowedCountries = new List<string> { "EG", "US" }
					}
				};

				foreach (var item in cart.OrderItems)
				{
					var sessionListItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmountDecimal = item.Price,
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product?.Name,
								Description = item.Product?.ShortDescription
							},
							Currency = "EGP"
						},
						Quantity = item.Quantity
					};
					options.LineItems.Add(sessionListItem);
				}

				try
				{
					var service = new SessionService();
					var session = await service.CreateAsync(options);
					TempData["SessionId"] = session.Id;
					Response.Headers.Append("Location", session.Url);
					return new StatusCodeResult(303);
				}
				catch (Exception ex)
				{
					// Handle exception appropriately (log, display error message, etc.)
					return StatusCode(500); // or return a specific error view
				}
			}

			return RedirectToAction("Index", "Cart");
		}


	}
}
