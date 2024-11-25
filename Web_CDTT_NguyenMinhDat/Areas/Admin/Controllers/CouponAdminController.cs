using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Areas.Admin.Controllers
{

    [Area("admin")]
    [Route("admin")]
    [Route("admin/home")]

    public class CouponAdminController : Controller
    {
        private readonly DataContext _dataContext;
        public CouponAdminController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("listcoupon")]
        public async Task<IActionResult> ListCoupon()
        {
            var couponlist= await _dataContext.Coupons.ToListAsync();
            ViewBag.CouponList = couponlist;
            
            return View();
        }

        [Route("addcoupon")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCoupon(CouponModel coupon)
        {


            if (ModelState.IsValid)
            {

                _dataContext.Add(coupon);
                await _dataContext.SaveChangesAsync();
                TempData["Message"] = "Thêm khuyến mãi thành công";
                return RedirectToAction("ListCoupon");
            }
            else
            {
                TempData["Message"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
        }
    }
}
