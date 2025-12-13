using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using System.Security.Claims;

namespace RushtonRoots.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAccountService accountService,
        ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _accountService.LoginAsync(model);

        if (result)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _accountService.ForgotPasswordAsync(model);
        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string? code = null)
    {
        if (code == null)
        {
            return BadRequest("A code must be supplied for password reset.");
        }

        var model = new ResetPasswordViewModel { Code = code };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _accountService.ResetPasswordAsync(model);
        if (result)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        ModelState.AddModelError(string.Empty, "Error resetting password.");
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var profile = await _accountService.GetUserProfileAsync(userId);
        if (profile == null)
        {
            return NotFound();
        }

        return View(profile);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Profile(UserProfileViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _accountService.UpdateUserProfileAsync(userId, model);
        if (result)
        {
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Profile));
        }

        ModelState.AddModelError(string.Empty, "Error updating profile.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            return BadRequest("User ID and code are required.");
        }

        var result = await _accountService.ConfirmEmailAsync(userId, code);
        if (result)
        {
            return View("ConfirmEmail");
        }

        return View("Error");
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,HouseholdAdmin")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _accountService.CreateUserAsync(request, userId);
        if (result)
        {
            TempData["SuccessMessage"] = "User created successfully. A confirmation email has been sent.";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Error creating user. Please check permissions and try again.");
        return View(request);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
