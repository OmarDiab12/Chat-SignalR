using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalR.Models.ViewModels;
using SignalR.Services;

namespace SignalR.Controllers
{
    public class AccountController(InMemoryUserStore userStore, ILogger<AccountController> logger) : Controller
    {
        private readonly InMemoryUserStore _userStore = userStore;
        private readonly ILogger<AccountController> _logger = logger;

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_userStore.TryAddUser(model.UserName, model.Password, out var error))
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to create account.");
                return View(model);
            }

            await SignInAsync(model.UserName);
            TempData["Success"] = "Account created successfully.";
            return RedirectToAction("ChatPage", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!_userStore.TryValidateUser(model.UserName, model.Password, out _))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            await SignInAsync(model.UserName);
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("ChatPage", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have been signed out.";
            return RedirectToAction("Login");
        }

        private async Task SignInAsync(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userName),
                new Claim(ClaimTypes.Name, userName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow
            });

            _logger.LogInformation("User {User} signed in.", userName);
        }
    }
}
