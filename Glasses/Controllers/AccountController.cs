using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Glasses.Data;
using Glasses.Models.ViewModels;
using Glasses.Models;

namespace Glasses.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyBanKinhDbContext _db;
        public AccountController(QuanLyBanKinhDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (await _db.NguoiDungs.AnyAsync(n => n.Email == vm.Email))
            {
                ModelState.AddModelError(nameof(vm.Email), "Email này đã được sử dụng");
                return View(vm);
            }

            string maND = "ND" + DateTime.Now.Ticks.ToString()[^8..];

            var NguoiDung = new NguoiDung
            {
                MaNguoiDung = maND,
                HoVaTen = vm.HoVaTen,
                Email = vm.Email,
                SoDienThoai = vm.SoDienThoai,
                MatKhau = BCrypt.Net.BCrypt.HashPassword(vm.Password),
                PhanQuyen = 2
            };

            _db.NguoiDungs.Add(NguoiDung);

            _db.KhachHangs.Add(new KhachHang { MaKhachHang = maND });

            await _db.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _db.NguoiDungs.FirstOrDefaultAsync(n => n.Email == vm.Email);

            if(user == null || !BCrypt.Net.BCrypt.Verify(vm.Password, user.MatKhau))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                return View(vm);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.MaNguoiDung),
                new(ClaimTypes.Name, user.HoVaTen),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, GetRoleName(user.PhanQuyen))
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = vm.RememberMe,
                    ExpiresUtc = vm.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(7)
                        : DateTimeOffset.UtcNow.AddHours(2)
                });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();

        private static string GetRoleName(int phanQuyen) => phanQuyen switch
        {
            0 => "QuanLy",
            1 => "NhanVien",
            2 => "KhachHang"
        };
    }
}
