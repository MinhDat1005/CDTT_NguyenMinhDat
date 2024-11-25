using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/home")]
    [Authorize]
    public class OrderAdminController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderAdminController(DataContext context)
        {
            _dataContext = context;
        }
        [Route("listorder")]
        public async Task<IActionResult> ListOrder()
        {
            var listOrders = await _dataContext.OrderModels
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return View(listOrders);
        }

        [Route("listorder/vieworder")]

        public async Task<IActionResult> ViewOrder(string ordercode)
        {

            var DetailOrder = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == ordercode).ToListAsync();
            //lấy shipping
            var ShippingCod = _dataContext.OrderModels.Where(o => o.OrderCode == ordercode).First();
            ViewBag.ShippingCod = ShippingCod.ShippingCOD;

            return View(DetailOrder);
        }

        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.OrderModels.FirstOrDefaultAsync(o => o.OrderCode == ordercode);

            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "update successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the order status");
            }
        }
    }
}

   
    

