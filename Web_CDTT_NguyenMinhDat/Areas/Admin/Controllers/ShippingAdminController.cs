using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/home")]
    public class ShippingAdminController : Controller
    {
        private readonly DataContext _dataContext;
        public ShippingAdminController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("listship")]
        public async Task <IActionResult> ListShip()
        {
            var shiplist = await _dataContext.Shippings.ToListAsync();
            ViewBag.shipping = shiplist;
            return View();
        }
        [Route("addship")]
        [HttpPost]

        public async Task<IActionResult> AddShipping(ShippingModel shippingModel,decimal price, string phuong, string quan, string tinh)
        {
            shippingModel.Ward = phuong;
            shippingModel.District = quan;
            shippingModel.City = tinh;
            shippingModel.Price=price;


            try
            {
                var existingShip = await _dataContext.Shippings.AnyAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

                if (existingShip)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp." });
                }
                _dataContext.Shippings.Add(shippingModel);
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Thêm ship thành công." });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding shipping");
            }
        }
        [Route("listship/delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            ShippingModel shippingModel = await _dataContext.Shippings.FindAsync(Id);

            _dataContext.Shippings.Remove(shippingModel);
            await _dataContext.SaveChangesAsync();
            TempData["Message"] = "Xóa ship thành công.";
            return RedirectToAction("ListShip");
        }
    }
}
