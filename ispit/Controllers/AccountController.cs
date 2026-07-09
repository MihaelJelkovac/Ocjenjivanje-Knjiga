using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Lab5.Models;
using Lab5.Services;
using Lab5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        _logger.LogInformation("📝 Pokušaj registracije: {Email}", model.Email);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("⚠️ Validacija nije prošla pri registraciji: {Email}", model.Email);
            return View(model);
        }

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            OIB = model.OIB,
            JMBG = model.JMBG
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("✅ Korisnik uspješno registriran: {Email}", model.Email);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        _logger.LogWarning("⚠️ Registracija nije uspjela za {Email}: {Errors}",
            model.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        _logger.LogInformation("🔑 Pokušaj prijave: {Email}", model.Email);

        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation("✅ Uspješna prijava: {Email}", model.Email);
            return RedirectToLocal(returnUrl);
        }

        _logger.LogWarning("⚠️ Neuspješna prijava: {Email}", model.Email);
        ModelState.AddModelError(string.Empty, "Neispravna prijava.");
        ViewBag.ReturnUrl = returnUrl;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        _logger.LogInformation("🚪 Odjava korisnika: {Email}", User.Identity?.Name);
        await _signInManager.SignOutAsync();
        return RedirectToLocal(returnUrl);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ExternalLogin(string provider = GoogleDefaults.AuthenticationScheme, string? returnUrl = null)
    {
        _logger.LogInformation("🔑 Pokretanje vanjske prijave preko: {Provider}", provider);
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            ModelState.AddModelError(string.Empty, remoteError);
            return View(nameof(Login));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            return RedirectToAction(nameof(Login));
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (signInResult.Succeeded)
        {
            _logger.LogInformation("✅ Uspješna vanjska prijava: {Provider}", info.LoginProvider);
            return RedirectToLocal(returnUrl);
        }

        var email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email)
            ?? $"external-{info.ProviderKey}@example.com";

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new AppUser
            {
                UserName = email,
                Email = email,
                OIB = GenerateNumericString(11),
                JMBG = GenerateNumericString(13)
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(nameof(Login));
            }
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        if (!addLoginResult.Succeeded)
        {
            foreach (var error in addLoginResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(nameof(Login));
        }

        _logger.LogInformation("✅ Novi korisnik kreiran preko vanjske prijave: {Email}", email);

        if (string.Equals(email, IdentitySeeder.DesignatedAdminEmail, StringComparison.OrdinalIgnoreCase)
            && !await _userManager.IsInRoleAsync(user, "Admin"))
        {
            await _userManager.AddToRoleAsync(user, "Admin");
            _logger.LogInformation("👑 Korisnik {Email} postavljen kao Admin", email);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToLocal(returnUrl);
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    private static string GenerateNumericString(int length)
    {
        var builder = new StringBuilder(length);
        for (var index = 0; index < length; index++)
        {
            builder.Append(RandomNumberGenerator.GetInt32(0, 10));
        }

        return builder.ToString();
    }
}