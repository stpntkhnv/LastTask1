﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LastTask1.ViewModels;
using LastTask1.Models;
using Microsoft.AspNetCore.Identity;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace CustomIdentityApp.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.UserName, nItems = 0, Role = "user", nCollections = 0, nComments = 0, nLikes = 0};
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }


        public IActionResult ChangeTheme(string returnUrl)
        {
            if (Request.Cookies["theme"] == null)
            {
                Response.Cookies.Append("theme", "dark", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            }
            else if (Request.Cookies["theme"] == "dark")
            {
                Response.Cookies.Delete("theme");
                Response.Cookies.Append("theme", "light", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            }
            else
            {
                Response.Cookies.Delete("theme");
                Response.Cookies.Append("theme", "dark", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            };
            return LocalRedirect(returnUrl);
        }


        public IActionResult GetCulture(string code, string returnUrl)
        {
            if (!String.IsNullOrEmpty(code))
            {
                CultureInfo.CurrentCulture = new CultureInfo(code);
                CultureInfo.CurrentUICulture = new CultureInfo(code);
            }
             return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password, bool remember)
        {
           
                var result =
                    await _signInManager.PasswordSignInAsync(userName, password, remember, false);
                if (result.Succeeded)
                {
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<User> GetUser(string userName)
        {
            User User = await _userManager.FindByNameAsync(userName);
            return User;
        }
    }
}