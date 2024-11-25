using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Models;

namespace Web_CDTT_NguyenMinhDat.Repository
{
	public class SeedData
	{
		public static void SeedingData(DataContext _context)
		{
			_context.Database.Migrate();
			if (!_context.Products.Any())
			{
				CategoryModel macbook = new CategoryModel { Name = "Macbook", Description = "Apple is large Brand in the word", Slug = "macbook", Status = 1 };
				CategoryModel pc = new CategoryModel { Name = "PC", Description = "Apple is large Brand in the word", Slug = "pc", Status = 1 };
				BrandModel Apple = new BrandModel { Name = "Apple", Description = "Apple is large Brand in the word", Slug = "apple", Status = 1 };
				BrandModel SamSung = new BrandModel { Name = "SamSung", Description = "Apple is large Brand in the word", Slug = "samsung", Status = 1 };
				_context.Products.AddRange(

				new ProductModel { Name = "Pc", Slug = "pc", Description = "Pc is best", Image = "2.jpg", Category = pc, Price = 123435, Brand = SamSung },
				new ProductModel { Name = "Macbook", Slug = "macbook", Description = "Microsoft is best", Image = "1.jpg", Category = macbook, Price = 12345, Brand = Apple }
				);
				_context.SaveChanges();

			}
		}

		
	}
}
