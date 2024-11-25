using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_CDTT_NguyenMinhDat.Repository.Validation;

namespace Web_CDTT_NguyenMinhDat.Models
{
	public class ProductModel
	{
		[Key]
		public int Id { get; set; }
        //public int Id { get; set; }


        [Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm"), MinLength(4)]
		public string Name { get; set; }

		public string Slug { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập mô tả sản phẩm"), MinLength(4)]
		public string Description { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập giá sản phẩm"), Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
		public decimal Price { get; set; }

		public int CategoryId { get; set; }
		public int BrandId { get; set; }

		public int Quantity { get; set; }
		public int Sold { get; set; }

		public string Image { get; set; }
		[NotMapped]
		[FileExtension]
		public IFormFile? ImageUpload { get; set; }

		public RatingModel RatingDetails { get; set; }
		public CategoryModel Category { get; set; }
		public BrandModel Brand { get; set; }
	
		
    }
}
