using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Web_CDTT_NguyenMinhDat.Models; 

namespace Web_CDTT_NguyenMinhDat.Repository.Components
{
	public class BrandsViewComponent : ViewComponent
	{
		private readonly DataContext _dataContext;

		public BrandsViewComponent(DataContext context)
		{
			_dataContext = context;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var brands = await _dataContext.Brands.ToListAsync();
			return View(brands);
		}
	}
}
