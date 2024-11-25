using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_CDTT_NguyenMinhDat.Repository.Validation;

namespace Web_CDTT_NguyenMinhDat.Models
{
	public class ContactModel
	{
		[Key]
		public string Name { get; set; }
		public string Description { get; set; }
		public string LogoImage { get; set; }
		[NotMapped]
		[FileExtension]
		public IFormFile ImageUpLoad { get; set; }
		public string Phone {  get; set; }
		public string Email { get; set; }
		public string Map { get; set; }
	}
}
