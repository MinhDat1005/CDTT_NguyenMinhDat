using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Controllers
{
	

	public class CategoryController : Controller
    {
		private readonly DataContext _dataContext;

		public CategoryController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
		public async Task<IActionResult> Index(string Slug = "")
		{
			var category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();

			if (category == null)
			{
				return RedirectToAction("Index");
			}

			var productsByCategory = await _dataContext.Products
				.Where(p => p.CategoryId == category.Id)
				.OrderByDescending(p => p.Id)
				.ToListAsync();

			return View(productsByCategory);
		}
	}
}
