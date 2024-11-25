using Web_CDTT_NguyenMinhDat.Models;

namespace Web_CDTT_NguyenMinhDat.Models.ViewModels
{
	public class ProductFilterViewModel
	{
		public IEnumerable<ProductModel> Products { get; set; }
		public IEnumerable<CategoryModel> Categories { get; set; }
		public IEnumerable<BrandModel> Brands { get; set; }
    
    }
}

