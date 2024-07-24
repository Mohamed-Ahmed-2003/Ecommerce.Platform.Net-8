using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.ViewModels;

namespace OtlobLap.Services
{
    public interface IBannerViewService
    {
        Task<IEnumerable<Banner>> GetCoverViewsAsync();

        Task AddCoverViewAsync(BannerVM coverView);
        Task UpdateCoverViewAsync(BannerVM coverView);
        Task DeleteCoverViewAsync(int coverViewId);
    }
    public class BannerViewService(AppDbContext context , IImageService imageService) : IBannerViewService { 
        private readonly AppDbContext _context = context;
        private readonly IImageService _imageService = imageService;

        public async Task AddCoverViewAsync(BannerVM coverView)
        {

            var newBanner = new Banner
            {
                Description = coverView.Description,
                Link = coverView.LinkUrl,
                ImageUrl = await _imageService.UploadPhysical(coverView.ImageFile, "Banners")
            };

            _context.Banners.Add(newBanner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCoverViewAsync(BannerVM coverView)
        {
            var targetBanner = await _context.Banners.FindAsync(coverView.Id);
            if (targetBanner == null) return;

              targetBanner.Description = coverView.Description;
              targetBanner.Link = coverView.LinkUrl;
            if (coverView.ImageFile != null)
            {
                 _imageService.RemoveImagePhysical("Banners", targetBanner.ImageUrl);
                targetBanner.ImageUrl = await _imageService.UploadPhysical(coverView.ImageFile, "Banners");
            }

            _context.Banners.Update(targetBanner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCoverViewAsync(int coverViewId)
        {
            var coverView = await _context.Banners.FindAsync(coverViewId);
            if (coverView != null)
            {
                _imageService.RemoveImagePhysical("Banners", coverView.ImageUrl);
                _context.Banners.Remove(coverView);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Banner>> GetCoverViewsAsync()
        {
            return await _context.Banners.ToListAsync();
        }
    }


}
