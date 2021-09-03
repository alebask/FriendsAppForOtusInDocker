using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FriendsAppNoORM.Data;
using FriendsAppNoORM.Models;
using FriendsAppNoORM.Utilities;
using FriendsAppNoORM.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FriendsAppNoORM.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDatabaseContext _dbContext;

        private readonly IPasswordHasher _passwordHasher;

        public AccountController(ILogger<AccountController> logger, ApplicationDatabaseContext context, IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _dbContext = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                Account a = _dbContext.Account.RerieveByEmail(model.Email);

                if (a != null)
                {                    
                    (bool verified, bool needUpgrade) = _passwordHasher.Check(a.PasswordHash, model.Password);
                    if(verified){

                        _logger.LogInformation($"User {model.Email} logged in");
                        
                        string accountName = null;
                                                
                        if(a.ProfileId != null){
                            accountName = _dbContext.Profile.Retrieve(a.ProfileId.Value).FirstName;                       
                        }
                        else
                        {
                            accountName = HttpContext.User.GetAccountEmail();
                        }

                        await HttpContext.SignInAsync(a.AccountId, accountName, a.ProfileId, model.RememberMe);

                        return RedirectToLocal(returnUrl);

                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Password is incorrect");
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, $"No account with email {model.Email}. Please register");
                }
            }
            
            return View(model);
        }

        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if (!_dbContext.Account.Exists(model.Email))
                {
                    Account input = new Account();                    
                    input.Email = model.Email;
                    input.PasswordHash = _passwordHasher.Hash(model.Password);

                    Account created = _dbContext.Account.Create(input);

                    _logger.LogInformation("User created a new account with password.");

                    await HttpContext.SignInAsync(created.AccountId, model.Email, created.ProfileId, model.RememberMe);

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, $"Account with email {model.Email} already exists. Try another email!");
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

    }
}
