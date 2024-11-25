using Microsoft.AspNetCore.Mvc;

namespace Web_CDTT_NguyenMinhDat.Areas.Admin.Controllers
{
    public class DashboardAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
