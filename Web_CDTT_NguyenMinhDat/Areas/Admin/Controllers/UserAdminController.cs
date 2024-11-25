using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Repository;

namespace Web_CDTT_NguyenMinhDat.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/home")]
    [Authorize(Roles="Admin")]
    public class UserAdminController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserAdminController(DataContext dataContext, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dataContext = dataContext;
        }
        //list
        [Route("listuser")]
        public async Task<IActionResult> ListUser()
        {
            var usersWithRoles = await _dataContext.Users
      .Join(_dataContext.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
      .Join(_dataContext.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new
      {
          User = ur.u,
          RoleName = r.Name
      }).ToListAsync();

            return View(usersWithRoles);
        }
        //adđ
        [HttpGet]
        [Route("listuser/add")]
        public async Task<IActionResult> Add()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }

        [HttpPost]
        [Route("listuser/add")]
        public async Task<IActionResult> Add(AppUserModel user)
        {
           if(ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if(createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email);
                    var userId = createUser.Id;
                    var role = _roleManager.FindByIdAsync(user.RoleId);
                    //gán quyền
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Result.Name);
                    if(!addToRoleResult.Succeeded)
                    {
                        foreach (var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }                 
                    return RedirectToAction("ListUser", "UserAdmin");
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                } 
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
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        //edit

        [HttpGet]
        [Route("listuser/eidt")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("listuser/eidt")]
        public async Task<IActionResult> Edit(string id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if(existingUser == null)
            {
                return NotFound();
            }    
            if(ModelState.IsValid)
            {
                existingUser.UserName = user.UserName;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Email = user.Email;
                existingUser.RoleId = user.RoleId;

                var updateUser = await _userManager.UpdateAsync(existingUser);
                if(updateUser.Succeeded)
                {
                    return RedirectToAction("ListUser", "UserAdmin");
                } else
                {
                    AddIdentityError(updateUser);
                    return View(existingUser);
                }    
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            TempData["Message"] = "Model có một vài thứ đang bị lỗi";
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            string errorMessage = string.Join("\n", errors);
            return BadRequest(existingUser);
        }

        private void AddIdentityError(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        //delete
        [HttpGet]
        [Route("listuser/delete")]
        public async Task <IActionResult> Delete( string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound(); 
            }
            var deleteresult = await _userManager.DeleteAsync(user);
            if(!deleteresult.Succeeded)
            {
                return View("Error");
            }
            TempData["Message"] = "Thêm User thành công !";
            return RedirectToAction("ListUser");
        }

    }
}
