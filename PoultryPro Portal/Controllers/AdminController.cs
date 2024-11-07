// Controllers/AdminController.cs
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using PoultryPro_Portal.Models;

using PoultryPro_Portal.Services;
using System.Threading.Tasks;

namespace PoultryPro_Portal.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        //private readonly FirestoreDb _fireStoreDb;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Signup()
        {
            if (await _adminService.IsAdminRegisteredAsync())
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(AdminModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the registration result
                var registerResult = await _adminService.RegisterAdminAsync(model.adminEmail, model.password);

                // Check the success of registration
                if (registerResult.Success)
                {
                    return RedirectToAction("AgentDashboard", "DashBoard");
                }

                // If registration fails, add the specific error message
                ModelState.AddModelError("", registerResult.ErrorMessage);
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminModel model)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _adminService.LoginAdminAsync(model.adminEmail, model.password);

                if (loginResult.Success)
                {
                    HttpContext.Session.SetString("AdminLoggedIn", "true");
                    return RedirectToAction("AgentDashboard", "DashBoard");

                }

                // Display specific error message from LoginResult
                ModelState.AddModelError("", loginResult.ErrorMessage);
            }
            return View(model);
           
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLoggedIn");
            return RedirectToAction("Login");
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View();
            }

            var isEmailSent = await _adminService.SendPasswordResetEmailAsync(email);
            if (isEmailSent)
            {
                ViewBag.Message = "Password reset email has been sent. Please check your inbox.";
            }
            else
            {
                ModelState.AddModelError("", "Failed to send password reset email. Please try again.");
            }
            return View();
        }
    }

}