using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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

    public class CategoryAdminController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryAdminController(DataContext context)
        {
            _dataContext = context;
        }

        [Route("listcategory")]
        public async Task<IActionResult> ListCategory()
        {
            var listCategories = await _dataContext.Categories
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return View(listCategories);
        }

        [HttpGet]
        [Route("listcategory/add")]
        public IActionResult Add()
        {
            return View();
        }

        [Route("listcategory/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryModel category)
        {
            

            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-").ToLower();
                var existingcategory = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);

                if (existingcategory != null)
                {
                    ModelState.AddModelError("", "Danh mục đã có trong database");
                    return View(category);
                }
                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["Message"] = "Thêm danh mục thành công";
                return RedirectToAction("Listcategory");
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
            return View(category);
        }

        //Update
        [Route("listcategory/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);
            return View(category);
        }

        [Route("listcategory/edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-").ToLower();

                var existingCategory = await _dataContext.Categories
                    .FirstOrDefaultAsync(p => p.Slug == category.Slug && p.Id != category.Id);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("", "Danh mục đã có trong database");
                    return View(category);
                }

                _dataContext.Update(category);
                await _dataContext.SaveChangesAsync();

                TempData["Message"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Listcategory");
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


        //delete
        [Route("listcategory/delete")]
        public async Task <IActionResult> Delete (int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["Message"] = "Xóa danh mục thành công !";
            return RedirectToAction("ListCategory");
        }
    }
}
