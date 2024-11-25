using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Web_CDTT_NguyenMinhDat.Models; // Ensure this namespace is correct

namespace Web_CDTT_NguyenMinhDat.Repository.Components
{
	public class CategoriesViewComponent : ViewComponent
	{
		private readonly DataContext _dataContext;

		public CategoriesViewComponent(DataContext context)
		{
			_dataContext = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var categories = await _dataContext.Categories.ToListAsync();
			return View(categories);
		}
	}
}
