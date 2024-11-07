using Microsoft.AspNetCore.Mvc;
using PoultryPro_Portal.Models.Call_Center_Agent;
using PoultryPro_Portal.Services;

namespace PoultryPro_Portal.Controllers
{
        public class AgentController : Controller
        {
            private readonly ICallCenterAgentService _userService;

            public AgentController(ICallCenterAgentService userService)
            {
                _userService = userService;
            }

            // Display Agent Signup page
            [HttpGet]
            public IActionResult Signup()
            {
                return View();
            }

            // Handle Agent Signup
            [HttpPost]
            public async Task<IActionResult> Signup(AgentModel model)
            {
                if (ModelState.IsValid)
                {
                    var isRegistered = await _userService.RegisterAgentAsync(model.callAgentEmail, model.password);
                    if (isRegistered.Success)
                    {
                        return RedirectToAction("Login");
                    }
                    ModelState.AddModelError("", "Failed to register agent. Please try again.");
                }
                return View(model);
            }

            // Display Agent Login page
            [HttpGet]
            public IActionResult Login()
            {
                return View();
            }

            // Handle Agent Login
            [HttpPost]
            public async Task<IActionResult> Login(AgentModel model)
            {
                if (ModelState.IsValid)
                {
                    var isLoggedIn = await _userService.LoginAgentAsync(model.callAgentEmail, model.password);
                    if (isLoggedIn.Success)
                    {
                        HttpContext.Session.SetString("AgentLoggedIn", "true");
                        return RedirectToAction("Dashboard");
                    }
                    ModelState.AddModelError("", "Invalid credentials.");
                }
                return View(model);
            }

            // Display Agent Dashboard (restricted to logged-in agents)
            public IActionResult Dashboard()
            {
                if (HttpContext.Session.GetString("AgentLoggedIn") != "true")
                {
                    return RedirectToAction("Login");
                }
                return View();
            }

            // Logout agent
            public IActionResult Logout()
            {
                HttpContext.Session.Remove("AgentLoggedIn");
                return RedirectToAction("Login");
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

            var isEmailSent = await _userService.SendPasswordResetEmailAsync(email);
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
