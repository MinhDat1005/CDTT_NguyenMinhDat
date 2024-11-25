using System.ComponentModel.DataAnnotations;

namespace Web_CDTT_NguyenMinhDat.Models.ViewModels
{
	public class ProductDetailViewModel
	{
		public ProductModel ProductDetail { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập comment"), MinLength(4)]
		public string Comment { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên"), MinLength(4)]

		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập email")]
		[EmailAddress]
		public string Email { get; set; }

       
    }
}
