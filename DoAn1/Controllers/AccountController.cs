using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DoAn1.Controllers;
using DoAn1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Internal;

namespace DoAn1.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext context;
        private readonly IMapper _mapper;
        private readonly UserManager<KhachHang> _userManager;
        private readonly SignInManager<KhachHang> _signInManager;
        public AccountController(IMapper mapper, UserManager<KhachHang> userManager, SignInManager<KhachHang> signInManager, DataContext context)
        {
            this.context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(KhachHangDangKiModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }
            else
            {
                var user = _mapper.Map<KhachHang>(userModel);
                user.UserName = userModel.Email;
                user.HinhAnh = "user.png";
                user.TenKH = userModel.FirstName + userModel.LastName;
                var result = await _userManager.CreateAsync(user, userModel.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return View(userModel);
                }
                else
                {
                    if (user.Id.Count() == 1) await _userManager.AddToRoleAsync(user, "Admin");
                    else await _userManager.AddToRoleAsync(user, "Customer");
                }
                context.KhachHang.AddRange(user);
                
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            KhachHangDangNhapModel model = new KhachHangDangNhapModel
            {
                ReturnURL = returnUrl,
                ExternalLogin = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(KhachHangDangNhapModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }
            var result = await _signInManager.PasswordSignInAsync(userModel.Email, userModel.Password, userModel.RememberMe, false);
            KhachHang us = await _userManager.FindByNameAsync(userModel.Email);
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(us, "Admin"))
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Email or Password");
                return View();
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Account", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var redirectUrl = Url.Action("ExternalLoginCallBack", "Account", new { ReturnUrl = returnUrl });

            KhachHangDangNhapModel model = new KhachHangDangNhapModel
            {
                ReturnURL = returnUrl,
                ExternalLogin = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError !=null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", model);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if(info==null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information");

                return View("Login", model);
            }

            var signinResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if(signinResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if(email!=null)
                {
                    var user = await _userManager.FindByEmailAsync(email);

                    if(user==null)
                    {
                        user = new KhachHang
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        };
                        user.HinhAnh = "user.png";
                        await _userManager.CreateAsync(user);
                    }

                    await _userManager.AddLoginAsync(user, info);
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }

            return View("Login", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            else return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}