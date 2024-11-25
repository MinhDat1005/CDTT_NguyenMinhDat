using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using Web_CDTT_NguyenMinhDat.Models;
using Web_CDTT_NguyenMinhDat.Models.ViewModels;
using Web_CDTT_NguyenMinhDat.Repository;
using Web_CDTT_NguyenMinhDat.Repository.SendMail;

namespace Web_CDTT_NguyenMinhDat.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly DataContext _dataContext;
        private readonly Repository.SendMail.IEmailSender _emailSender;
        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, DataContext dataContext, Repository.SendMail.IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dataContext = dataContext;
            _emailSender = emailSender;
        }

        public IActionResult Login( string returnUrl)
        {
            return View(new LoginViewModel
            {
                returnUrl = returnUrl,
            });
        }
        //-------------------------------
        //forgotpass
        public async Task<IActionResult> SendMailForgotPass(AppUserModel user)
        {
            var checkmail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (checkmail == null)
            {
                TempData["Message"] = "Email not found";
                return RedirectToAction("ForgotPassword", "Account");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(checkmail);
            checkmail.Token = token;  

            _dataContext.Update(checkmail);
            await _dataContext.SaveChangesAsync();

            var receiver = checkmail.Email;
            var subject = "Change password for user " + checkmail.Email;

            var encodedEmail = WebUtility.UrlEncode(checkmail.Email);
            var encodedToken = WebUtility.UrlEncode(token);
            var resetLink = $"{Request.Scheme}://{Request.Host}/Account/NewPass?email={encodedEmail}&token={encodedToken}";

            var message = $"Click on the link to change your password: <a href='{resetLink}'>Reset Password</a>";

            try
            {
                await _emailSender.SendEmailAsync(receiver, subject, message, "MDShop", "datminh100504@gmail.com");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error sending email. Please try again later.";
                return RedirectToAction("ForgotPassword", "Account");
            }

            TempData["Message"] = "Please check your email for the password reset link.";
            return RedirectToAction("ForgotPassword", "Account");
        }


        //---------------------------
        public async Task<IActionResult> ForgotPassword(string returnUrl)
        {
            return View();
        }
        //----------------------
        public async Task<IActionResult> NewPass(AppUserModel appUser, string token)
        {
            var checkuser = await _userManager.Users
                 .Where(u => u.Email == appUser.Email)
                 .Where(u => u.Token == token)
                 .FirstOrDefaultAsync();

            if (checkuser != null)
            {
                // User found; pass email and token to ViewBag for form use
                ViewBag.Email = checkuser.Email;
                ViewBag.Token = token;
            }
            else
            {
                // Redirect to ForgotPassword if the user or token is invalid
                return RedirectToAction("ForgotPassword", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNewPassword(AppUserModel appUser, string token)
        {
            var checkuser = await _userManager.Users
                  .Where(u => u.Email == appUser.Email)
                  .Where(u => u.Token == token)
                  .FirstOrDefaultAsync();

            if (checkuser != null)
            {
                // Generate a new token and hash the new password
                string newtoken = Guid.NewGuid().ToString();
                var passwordhasher = new PasswordHasher<AppUserModel>();
                checkuser.PasswordHash = passwordhasher.HashPassword(checkuser, appUser.PasswordHash);
                checkuser.Token = newtoken;

                await _userManager.UpdateAsync(checkuser);
                TempData["Message"] = "Password updated successfully";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["Message"] = "Email not found or token is incorrect";
                return RedirectToAction("ForgotPassword", "Account");
            }
        }
        //-----------------------

        [HttpPost]
        public async Task <IActionResult> Login(LoginViewModel loginVM)
        {
            if(ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName,loginVM.Password,false,false);
                if(result.Succeeded)
                {
                    return Redirect(loginVM.returnUrl ?? "/");
                }
                ModelState.AddModelError("", "Username or Password sai !");
            }
            return View(loginVM);
        }


        public async Task<IActionResult> Register(UserModel user)
        {
            if (ModelState.IsValid)
            {
                AppUserModel newUser = new AppUserModel {Email = user.Email, UserName = user.UserName };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Đăng kí thành công !";
                    return Redirect("/account/login");
                }
                foreach (IdentityError error in result.Errors) {
                    ModelState.AddModelError("", error.Description);
                }
            }    
            return View();
        }

        public async Task <IActionResult> Logout( string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();


            return Redirect(returnUrl);
        }

        //-------------------------------

        public async Task LoginGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            // Thêm tham số yêu cầu chọn lại tài khoản
            properties.Items["prompt"] = "select_account";

            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
        }


        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            string emailName = email.Split('@')[0];
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                var newUser = new AppUserModel { UserName = emailName, Email = email };
                var passwordHasher = new PasswordHasher<AppUserModel>();

                // Hash password with the user instance
                newUser.PasswordHash = passwordHasher.HashPassword(newUser, "123456789");

                var createUserResult = await _userManager.CreateAsync(newUser);
                if (!createUserResult.Succeeded)
                {
                    TempData["Message"] = "Đăng nhập tài khoản Google thất bại!";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    TempData["Message"] = "Đăng nhập tài khoản Google thành công!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                TempData["Message"] = "Đăng nhập tài khoản Google thành công!";
                return RedirectToAction("Index", "Home");
            }
        }

        
    }
}
