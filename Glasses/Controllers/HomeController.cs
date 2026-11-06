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
    }
}