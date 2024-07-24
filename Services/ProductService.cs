using Microsoft.EntityFrameworkCore;

using OtlobLap.Data;
using OtlobLap.Models;
using OtlobLap.ViewModels;


namespace OtlobLap.Services
{

    public interface IProductService
    {
        Task<List<Product>> GetProductsPageAsync(int page);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
		Task<List<Product>> GetProductsBySearch(string input,int page);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
        Task<List<Product>> GetProductsBySellerIdAsync(int sellerId);
         Task DeleteProductImages(Product product);
         Task DeleteProductImage(ProductImage Image);
        Task<List<Product>?> GetSimilarProductsAsync(Product product, int page);

        Task<List<Product>> GetProductsWithTag(int page, ProductsTag? tag);

        Task<int> ProductsCount(ProdViewSettingsDto settingsDto);
        Task<List<Product>> GetProductsWithOptionsAsync (ProdViewSettingsDto settingsDto);


		Task RemoveProductCategories(int id);
        Task<bool> CustomerHasBrought(int customerId, int ProductId);
        Task ToggleHideProduct(int Id);
        Task TogglePremiumProduct(int Id);
		int ProductsPageSize { get; }

	}

    public class ProductService(AppDbContext context ,IPaginationService paginationService  ) : IProductService
    {
        private readonly AppDbContext _context = context;
        private readonly IPaginationService _paginationService = paginationService;

        private IQueryable<Product> ProductsQuery()

           =>  _context.Products
            .Include(p => p.Seller)
            .Include(p => p.Discount)
            .Include(p => p.Reviews)
            .Include(p => p.Brand)
            .Include(P => P.ProductImages)
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category);
        
        public async Task<List<Product>> GetProductsPageAsync(int page)
        {
            int pageSize = ProductsPageSize ;
            
            var products = await ProductsQuery()
				.Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

               return products;
        }

		public async Task<Product?> GetProductByIdAsync(int productId)
		{
			var product = await ProductsQuery()
                .Include(P=>P.Reviews)
                .ThenInclude(R=>R.Customer)
				.IgnoreQueryFilters()
                .AsNoTracking()
				.FirstOrDefaultAsync(p => p.ID == productId);

			return product;
		}

		public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> GetProductsBySellerIdAsync(int sellerId)
        {
            return await ProductsQuery()
                .Where(p => p.SellerID == sellerId)
                .ToListAsync();
        }

        private IQueryable<Product> GetProductsBySearch(string input)
        {
			var query = ProductsQuery();

            return  query.Where(p => p.Name.Contains(input) || p.Seller.Name.Contains(input) || p.Brand.Name == input || p.ShortDescription.Contains(input));
        }

        public async Task DeleteProductImages(Product product)
        {
            _context.ProductImages.RemoveRange(product.ProductImages);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductImage(ProductImage Image)
        {
            _context.ProductImages.Remove(Image);
            await _context.SaveChangesAsync();
        }
     

        public  async Task<List<Product>?> GetSimilarProductsAsync(Product product, int page = 1 )
        {
            int pageSize = _paginationService.SimilarProductsPageSize;

            var categoriesId = product.ProductCategories?.Select(pc => pc.CategoryId);
            if (categoriesId is null || !categoriesId.Any()) return null;

            var similarProducts = await ProductsQuery()
								.Where(p => p.ID != product.ID && p.ProductCategories.Any(pc=> categoriesId.Contains(pc.CategoryId)) )
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();



            return similarProducts;

        }

        public async Task RemoveProductCategories(int id)
        {
            var productCategoryIds = await _context.ProductCategory
                .Where(pc => pc.ProductId == id)
                .Select(pc => pc.CategoryId)
                .ToListAsync();

            if (productCategoryIds == null || !productCategoryIds.Any())
                return;

            var productCategoriesToRemove = await _context.ProductCategory
                .Where(pc => pc.ProductId == id)
                .ToListAsync();

            _context.ProductCategory.RemoveRange(productCategoriesToRemove);
            await _context.SaveChangesAsync();
        }


		public async Task<bool> CustomerHasBrought(int customerId, int ProductId)
		{
            return ( await _context.OrderItems.FirstOrDefaultAsync(o=>o.ProductId == ProductId && o.CustomerId == customerId /* && o.Status == null )*/ )) != null;
		}

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await ProductsQuery()
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        .ToListAsync();

        }

        public async Task ToggleHideProduct(int Id)
        {
            var target = await GetProductByIdAsync(Id);
            if (target != null) target.IsHidden = !target.IsHidden;
            _context.Entry(target).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        } 
        
        public async Task TogglePremiumProduct(int Id)
        {
            var target = await GetProductByIdAsync(Id);
            if (target != null) target.Premium = !target.Premium;
            _context.Entry(target).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        private IQueryable<Product> SortProductsBy(IQueryable<Product> query, ResultsFilter? filter)
        {
            switch (filter)
            {
                case ResultsFilter.PriceOrderDesc:
                    return query.OrderByDescending(p => p.Price);

                case ResultsFilter.PriceOrderAsc:
                    return query.OrderBy(p => p.Price);

                case ResultsFilter.DateAdded:
                    return query.OrderByDescending(p => p.AddedDate);

                case ResultsFilter.AverageRating:

                    return query.OrderByDescending(p =>p.Reviews.Average(r=>r.Rating) );

                default:
                    return query;
            }
        }



        public int ProductsPageSize => _paginationService.ProductsPageSize ;
		public async Task<List<Product>> GetProductsBySearch(string input, int page)
		{
            int pageSize = ProductsPageSize;
			var query = GetProductsBySearch(input);
            return await _paginationService.ApplyPagination(query, page, pageSize).ToListAsync();
		}

        public async Task<List<Product>> GetProductsWithTag(int page, ProductsTag? tag)
        {
            int pageSize = ProductsPageSize;

			// Get queryable products
			var query = ProductsQuery();

            // Apply condition based on the tag
            query = ApplyTag(query, tag);
            // Apply pagination and retrieve products
            var products = await _paginationService.ApplyPagination(query, page, pageSize).ToListAsync();

            return products;
        }
        private IQueryable<Product> ApplyTag(IQueryable<Product> query, ProductsTag? tag)
        {
            switch (tag)
            {
                case ProductsTag.NewArrivals:
                    return NewestProductsTag(query);
                case ProductsTag.BestSellers:
                    return BestSellerProductsTag(query);
                case ProductsTag.Recommended:
                    return PremiumProductsTag(query);
                case ProductsTag.TopOffers:
                    return TopOffersProductsTag(query);
                default:
                    return query;
            }
        }

        public async Task<int> ProductsCount(ProdViewSettingsDto settingsDto)
        {
            var query = BuildProductQueryWithOptions(settingsDto);
            return await query.CountAsync();
        }

        public async Task<List<Product>> GetProductsWithOptionsAsync(ProdViewSettingsDto settingsDto)
        {
            var query = BuildProductQueryWithOptions(settingsDto);
            query = _paginationService.ApplyPagination(query, settingsDto.Page, _paginationService.ProductsPageSize);

            return await query.ToListAsync();
        }

        private IQueryable<Product> BuildProductQueryWithOptions(ProdViewSettingsDto settingsDto)
        {
            var query = string.IsNullOrEmpty(settingsDto.SearchKey) ? ProductsQuery() : GetProductsBySearch(settingsDto.SearchKey);

            if (settingsDto.Tag != null)
                query = ApplyTag(query, settingsDto.Tag);
            if (settingsDto.BrandId > 0)
            {
                query = query.Where(p => p.BrandId == settingsDto.BrandId);
            }
            if (settingsDto.CategoryId > 0)
            {
                query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryId == settingsDto.CategoryId));
            }
            if (settingsDto.Filter != null)
                query = SortProductsBy(query, settingsDto.Filter);


            return query;
        }


        #region Products Tag Delegates

        public IQueryable<Product> PremiumProductsTag(IQueryable<Product> query)
        {
            return query.Where(p => p.Premium);
        }

        public IQueryable<Product> BestSellerProductsTag(IQueryable<Product> query)
        {
            return query.OrderByDescending(p => p.OrderItems.Count());
        }

        public IQueryable<Product> NewestProductsTag(IQueryable<Product> query)
        {
            return query.Where(p => p.AddedDate.AddDays(7) >= DateTime.Now);
        }

        public IQueryable<Product> TopOffersProductsTag(IQueryable<Product> query)
        {
            return query.Include(p => p.Discount)
                        .Where(p => p.Discount != null && p.Discount.Percentage > 10);
        }


        #endregion


    }


}
