using Glasses.Data;
using Glasses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Glasses.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuanLyBanKinhDbContext _context;

        public HomeController(ILogger<HomeController> logger, QuanLyBanKinhDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sanPhamGiamGia = await _context.SanPhams
                                               .Where(s => s.PhanTramGiam > 0)
                                               .OrderBy(s => s.GiaDaGiam)
                                               .Take(10)
                                               .ToListAsync();

            return View(sanPhamGiamGia);
        }

        [Authorize]
        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string fullName, string email, string phone, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                ViewBag.ErrorMessage = "Vui lòng điền đầy đủ Họ tên, Email và Nội dung tin nhắn. ";
                return View();
            }
            ViewBag.SuccessMessage = $"Cảm ơn {fullName}! Chúng tôi đã nhận được yêu cầu tư vấn của bạn và sẽ phản hồi sớm nhất.";
            ModelState.Clear();
            return View();
        }
    }
}