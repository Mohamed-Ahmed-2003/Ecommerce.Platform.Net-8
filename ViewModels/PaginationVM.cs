namespace OtlobLap.ViewModels
{
	public class PaginationVM
	{
		public int CurrentPage { get; set; }
		public int PageSize { get; set; }
		public int TotalItems { get; set; }

		public int TotalPages
		{
			get
			{
				return (int)Math.Ceiling((double)TotalItems / PageSize);
			}
		}
	}
}
