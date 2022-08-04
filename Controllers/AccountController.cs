using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using AuthApp.Data;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        // private readonly AppDbContext _context;
        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender
        // AppDbContext context

        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            // _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: true
                );
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                if (result.Succeeded)
                {
                    return LocalRedirect(returnurl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                // var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email);

                if (user == null)
                {
                    return RedirectToAction("ForgotPasswordConfirm");
                }

                var code1 = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new
                    {
                        userId = user.Id,
                        code = code1
                    },
                    protocol: HttpContext.Request.Scheme
                );

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Reset Password",
                    "Please reset you password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string? code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }
                AddErrors(result);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null) {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register(string? returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new Models.User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var code1 = _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new
                        {
                            userId = user.Id,
                            code = code1
                        },
                        protocol: HttpContext.Request.Scheme
                    );

                    await _emailSender.SendEmailAsync(
                        model.Email,
                        "Confirm Your Account",
                        "Please confirm your account by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnurl);
                }
                AddErrors(result);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}