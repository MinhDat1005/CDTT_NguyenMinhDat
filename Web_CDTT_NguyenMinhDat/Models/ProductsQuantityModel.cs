using System.ComponentModel.DataAnnotations;

namespace Web_CDTT_NguyenMinhDat.Models
{
	public class ProductsQuantityModel
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		[Required(ErrorMessage ="Yêu cầu không bỏ trống")]
		public int Quantity { get; set; }

		public DateTime DateCreated { get; set; }
	}
}
