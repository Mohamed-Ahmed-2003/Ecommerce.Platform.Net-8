using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.ViewModels;
using static OtlobLap.Data.Utility.Permissions;

namespace OtlobLap.Services
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAllBrandsAsync();
        Task<Brand> GetBrandByIdAsync(int id);
        Task AddBrandAsync(CatalogVM brand);
        Task UpdateBrandAsync(CatalogVM brand);
        Task DeleteBrandAsync(int id);
    }

    public class BrandService(AppDbContext context, IImageService imageService) : IBrandService
    {
        private readonly AppDbContext _context = context;
        private readonly IImageService _imageService = imageService;

        public async Task AddBrandAsync(CatalogVM brandVM)
        {
            var newBrand = new Brand
            {
                Name = brandVM.Name,
            };

            if (brandVM.ImageFile != null)
            newBrand.BrandImage = await _imageService.UploadPhysical(brandVM.ImageFile, "Brands");

            _context.Brands.Add(newBrand);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBrandAsync(CatalogVM brandVM)
        {
            var brand = await GetBrandByIdAsync(brandVM.Id);

            if (brand is null) return;

            brand.Name = brandVM.Name;
            if (brandVM.ImageFile != null)
            {

                if (!string.IsNullOrEmpty(brand.BrandImage))
                    _imageService.RemoveImagePhysical("Brands", brand.BrandImage);
                
                brand.BrandImage = await _imageService.UploadPhysical(brandVM.ImageFile, "Brands");
            }
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBrandAsync(int brandId)
        {
            var brand = await _context.Brands.FindAsync(brandId);
            if (brand != null)
            {

            if (brand.BrandImage != null)
                _imageService.RemoveImagePhysical("Brands", brand.BrandImage);

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> GetBrandByIdAsync(int Id)
        {

            var brand = await _context.Brands.FindAsync(Id);
            return brand;
        }

  
    }
}
