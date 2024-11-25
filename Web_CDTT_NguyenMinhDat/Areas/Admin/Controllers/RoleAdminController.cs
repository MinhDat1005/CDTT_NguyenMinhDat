using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/home")]
    //[Authorize(Roles = "Admin")]
    public class RoleAdminController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        public RoleAdminController(DataContext context, RoleManager<IdentityRole> roleManager)
        {
            _dataContext = context;
            _roleManager = roleManager;
        }
        [HttpGet]
        [Route("listrole")]
        public async Task<IActionResult> ListRole()
        {
            var listRole = await _dataContext.Roles
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return View(listRole);
        }

        //adđ
        [HttpGet]
        [Route("listrole/add")]
        public async Task<IActionResult> Add()
        {
            return View();
        }

        [Route("listrole/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("ListRole");
        }
        //update
        [HttpGet]
        [Route("listrole/edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound();  
            }

            var role = await _roleManager.FindByIdAsync(id);
            
            return View(role);
        }

        [HttpPost]
        [Route("listrole/edit")]
        public async Task<IActionResult> Edit(string id, IdentityRole model)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if( role==null)
                {
                    return NotFound();

                }
                role.Name = model.Name;
                try {
                    await _roleManager.UpdateAsync(role);
                    TempData["Message"] = "Cập nhật role thành công !";
                    return RedirectToAction("ListRole");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while deleting the role!");
                }
                
            }
            return View(model ?? new IdentityRole
            {
                Id = id
            });
        }


            //delete
            [HttpGet]
        [Route("listrole/delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            try
            {
                await _roleManager.DeleteAsync(role);
                TempData["Message"] = "Xóa role thành công!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the role!");
            }

            return RedirectToAction("ListRole");
        }

    }
}
