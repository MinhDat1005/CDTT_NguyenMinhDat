﻿using System.ComponentModel.DataAnnotations;

namespace Web_CDTT_NguyenMinhDat.Models
{
	public class BrandModel
	{
		[Key]
		public int Id { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập thương hiệu")]
		public string Name { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả")]
		public string Description { get; set; }
		public string Slug { get; set; }
		public int Status { get; set; }
	}
}
