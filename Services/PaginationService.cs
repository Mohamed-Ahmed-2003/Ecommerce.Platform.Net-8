using Microsoft.Extensions.Options;
using OtlobLap.Configuration;
using OtlobLap.Models;

namespace OtlobLap.Services
{
	public interface IPaginationService
	{
		int ProductsPageSize { get; }
		int SimilarProductsPageSize { get; }
		IQueryable<Product> ApplyPagination(IQueryable<Product> query, int page, int pageSize);
		

	}
	public class PaginationService (IOptions<ProductsViewOptions> viewOptions) : IPaginationService
	{
		private readonly ProductsViewOptions _viewOptions = viewOptions.Value;

		public int ProductsPageSize => _viewOptions.ProductsPageSize;

		public int SimilarProductsPageSize => _viewOptions.SimilarProductsPageSize;

		public IQueryable<Product> ApplyPagination(IQueryable<Product> query, int page, int pageSize)
		{
			return query.Skip((page - 1) * pageSize)
						.Take(pageSize);
		}
	}
}
