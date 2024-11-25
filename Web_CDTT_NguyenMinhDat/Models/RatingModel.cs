using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_CDTT_NguyenMinhDat.Models
{
	public class RatingModel
	{
		[Key]
		public int Id { get; set; }
		public int ProductId { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập comment"), MinLength(4)]
		public string Comment { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập tên"), MinLength(4)]

		public string Name { get; set; }
		[Required(ErrorMessage = "Yêu cầu nhập email")]
		[EmailAddress]
		public string Email { get; set; }
		public string Star { get; set; }
		

		[ForeignKey("ProductId")]
		public ProductModel Product { get; set; }
	}
}
