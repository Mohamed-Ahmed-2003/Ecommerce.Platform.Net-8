using OtlobLap.Models;

namespace OtlobLap.ViewModels
{
	public class SearchVM
	{
		public required string Keyword { get; set; }
		public required ProductsResultsVM Results { get; set; }
		public ProdViewSettingsDto? SettingsDto { get; set; }
	}
}
