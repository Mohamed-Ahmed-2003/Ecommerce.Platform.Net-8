using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtlobLap.Controllers;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.Services;
using OtlobLap.ViewModels;

namespace OtlobLap.Areas.Admin.Controllers
{
    [Area(nameof(Utility.Roles.Admin))]
    [Authorize(Roles = nameof(Utility.Roles.Admin))]
    public class HomeController(IProfileService profileService, IAdminHomeService adminHomeService , IBannerViewService bannerViewService) : BaseController(profileService)
    {
        private readonly IAdminHomeService _adminHomeService = adminHomeService;
        private readonly IBannerViewService _bannerViewService = bannerViewService;
        public async Task<IActionResult> Index()
        {
            var model = await _adminHomeService.GetAdminInfoAsync();

            ViewBag.Controller = Utility.GetTranslation("AdminDashboard");
            ViewBag.view = Utility.GetTranslation("Data");

            return View(model);
        }
        public async Task<IActionResult> ManageBanners()
        {
            var model = new ManageBannersVM
            {
                Banners = (await _bannerViewService.GetCoverViewsAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddBanner (ManageBannersVM model)
        {
           
            if (model.BannerVM != null)
            {
                try
                {
                 await _bannerViewService.AddCoverViewAsync(model.BannerVM);
                return RedirectToAction("ManageBanners");
                }
                catch
                {
                    return View("ManageBanners", model);
                }
            }

            return View("ManageBanners", model);
        }

        public async Task<IActionResult> DeleteBanner (int bannerId)
        {
            await _bannerViewService.DeleteCoverViewAsync(bannerId);
            return RedirectToAction("ManageBanners");
        }
        [HttpPost]
        public async Task<IActionResult> EditBanner (ManageBannersVM model)
        {
           
            if (model.BannerVM != null)
            {
                try
                {
                 await _bannerViewService.UpdateCoverViewAsync(model.BannerVM);
                return RedirectToAction("ManageBanners");
                }
                catch
                {
                    return View("ManageBanners", model);
                }
            }

            return  View("ManageBanners", model);

        }

    }
}
