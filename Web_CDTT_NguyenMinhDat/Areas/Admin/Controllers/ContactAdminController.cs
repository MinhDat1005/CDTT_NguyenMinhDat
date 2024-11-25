using Microsoft.AspNetCore;
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
	public class ContactAdminController : Controller
	{
		private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactAdminController(DataContext context, IWebHostEnvironment webHostEnvironment)
		{
			_dataContext = context;
            _webHostEnvironment = webHostEnvironment;
		}
        [Route("listcontact")]
        public IActionResult ListContact()
        {
			var contact = _dataContext.Contacts.ToList();
            
            return View(contact);
        }
        [Route("listcontact/edit")]
		[HttpGet]
        public async Task <IActionResult> Edit()
		{
			ContactModel contact = await _dataContext.Contacts.FirstOrDefaultAsync();
			return View(contact);
		}
        [HttpPost]
        [Route("listcontact/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            // Tìm sản phẩm dựa vào Id
            var existingContact =  _dataContext.Contacts.FirstOrDefault();
          
            if (ModelState.IsValid)
            {
                existingContact.Name = contact.Name;
                existingContact.Description = contact.Description;
                existingContact.LogoImage = contact.LogoImage;
                existingContact.Phone = contact.Phone;
                existingContact.Email = contact.Email;

                // Xử lý cập nhật ảnh
                if (contact.ImageUpLoad != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media","logo");
                    string imageName = Guid.NewGuid().ToString() + "_" + contact.ImageUpLoad.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    string oldFilePath = Path.Combine(uploadsDir, existingContact.LogoImage);

                    try
                    {
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occurred while deleting the product image.");
                    }

                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        await contact.ImageUpLoad.CopyToAsync(fs);
                    }

                    existingContact.LogoImage = imageName;
                }

                _dataContext.Update(existingContact);
                await _dataContext.SaveChangesAsync();

                TempData["Message"] = "Cập nhật thành công";
                return RedirectToAction("ListProduct");
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
