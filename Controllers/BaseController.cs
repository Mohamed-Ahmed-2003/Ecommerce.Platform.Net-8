using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OtlobLap.Services;

namespace OtlobLap.Controllers
{
    public class BaseController ( IProfileService profileService) : Controller
    {
        private readonly IProfileService _profileService = profileService;


       

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var currUser = await _profileService.GetUserAsync();
                if (currUser == null)
                {
                    foreach (var cookie in Request.Cookies.Keys)
                    {
                        Response.Cookies.Delete(cookie);
                    }
                    RedirectToAction("Login", "Account",new {are = ""});
                    return;
                }

                string userImage = string.Empty;
                if (currUser.UserImage != null)
                {
                    userImage = $"data:image/png;base64,{Convert.ToBase64String(currUser.UserImage)}";
                }

                ViewBag.UserImage =string.IsNullOrEmpty(userImage) ? "/images/Users/noImage.png" : userImage;

              

                ViewBag.UserId = currUser.Id;
                ViewBag.UserName = currUser.Name;
                ViewBag.Role = currUser.Role;
			}
                ViewBag.Dir = "ltr";
                var rgf = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                if (rgf != null && rgf.RequestCulture.Culture.Name.StartsWith("ar")) {
                    Console.WriteLine(rgf.RequestCulture.Culture.Name);

				ViewBag.Dir = "rtl";}

            await base.OnActionExecutionAsync(context, next);
        }
    }

}

