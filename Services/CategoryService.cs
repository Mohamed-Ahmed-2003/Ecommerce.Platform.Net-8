using Microsoft.EntityFrameworkCore;
using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.ViewModels;

namespace OtlobLap.Services
{

    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<Category>> GetMostFilledCategoriesAsync();

        Task<List<Category>> GetAllCategoriesExcept (List<ProductCategory> categories);
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task AddCategoryAsync(CatalogVM categoryVM);
        Task UpdateCategoryAsync(CatalogVM categoryVM);
        Task DeleteCategoryAsync(int categoryId);
        Task<CategoryProductsVM> GetCategoryProductsAsync(int categoryId , int page);

    }

    public class CategoryService(AppDbContext context , IImageService imageService, IPaginationService paginationService) : ICategoryService
    {
        private readonly AppDbContext _context = context;
        private readonly IImageService _imageService = imageService;
		private readonly IPaginationService _paginationService = paginationService;

		public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task AddCategoryAsync(CatalogVM categoryVM)
        {
            var newCategory = new Category
            {
                Name = categoryVM.Name,
            };
            if (categoryVM.ImageFile != null)
                newCategory.BannerUrl = await _imageService.UploadPhysical(categoryVM.ImageFile, "Categories");

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(CatalogVM categoryVM)
        {
            var category = await GetCategoryByIdAsync(categoryVM.Id);

            if (category is null) return;

            category.Name = categoryVM.Name;
            if (categoryVM.ImageFile != null) {

                if (!string.IsNullOrEmpty(category.BannerUrl))
                 _imageService.RemoveImagePhysical("Categories", category.BannerUrl);

                category.BannerUrl = await _imageService.UploadPhysical(categoryVM.ImageFile, "Categories");
            }
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                if (category.BannerUrl != null)
                    _imageService.RemoveImagePhysical("Categories", category.BannerUrl);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        

        public Task<List<Category>> GetAllCategoriesExcept(List<ProductCategory> PC)
        {
            if (PC == null || !PC.Any())
                return GetAllCategoriesAsync();

            var categoryIds = PC.Select(pc => pc.CategoryId);
            return _context.Categories.Where(c => !categoryIds.Contains(c.ID)).ToListAsync();
        }

        public async Task<CategoryProductsVM> GetCategoryProductsAsync(int categoryId , int page)
        {
            var query =  _context.ProductCategory
                                        .Where(pc => pc.CategoryId == categoryId)
                                        .Include(pc => pc.Product)
                                        .ThenInclude(p => p.Brand)
                                        .Include(pc => pc.Product)
                                        .ThenInclude(p => p.Reviews)
                                        .Select(pc => pc.Product);

                         var products = await  _paginationService.ApplyPagination(query, page,_paginationService.ProductsPageSize).ToListAsync();             

            var category = await _context.Categories.FindAsync(categoryId);
            if (category is null) throw new Exception("Category is not exist");

            var productsResults = new ProductsResultsVM
            {
                Products = products,
                Brands = products.Where(p=>p.Brand != null).Select(p => p.Brand).ToHashSet(),
                Categories = products.Where(p => p.ProductCategories != null).SelectMany(p => p.ProductCategories.Select(p => p.Category)).OrderBy(c => c.Name).ToHashSet(),
			};

            var VM = new CategoryProductsVM { Results = productsResults, Category = category };

            return VM;
        }

		public async Task<List<Category>> GetMostFilledCategoriesAsync()
		{
            return await _context.Categories.Include(c=>c.ProductCategories)
                .Where(c=> !string.IsNullOrEmpty(c.BannerUrl))
                .OrderByDescending(c => c.ProductCategories.Count)
                .Take(6).ToListAsync();
		}
	}

}
