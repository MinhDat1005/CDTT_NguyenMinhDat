using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            var brands = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();

            if (brands == null)
            {
                return RedirectToAction("Index");
            }

            var productsByBrand = await _dataContext.Products
                .Where(p => p.BrandId == brands.Id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(productsByBrand);
        }
    }
}
