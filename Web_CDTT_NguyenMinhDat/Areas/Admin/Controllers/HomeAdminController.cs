using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_CDTT_NguyenMinhDat.Repository;
//using X.PagedList;

namespace WebCDTT_NguyenMinhDat.Areas.Admin.Controllers
{
  
    [Area("admin")]
    [Route("admin")]
	[Route("admin/home")]
    public class HomeAdminController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeAdminController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("")]
        public IActionResult Index()
        {
            var count_product = _dataContext.Products.Count();
            var count_order = _dataContext.OrderModels.Count();
            var count_category = _dataContext.Categories.Count();
            var count_user = _dataContext.Users.Count();

            ViewBag.CountProduct = count_product;
            ViewBag.CountOrder = count_order;
            ViewBag.CountCategory= count_category;
            ViewBag.CountUser = count_user;


            return View();
        }
    }
}
