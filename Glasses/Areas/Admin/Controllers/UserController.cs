using Glasses.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glasses.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly QuanLyBanKinhDbContext _context;

        public UserController(QuanLyBanKinhDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> UserIndex()
        {
            var users = await _context.NguoiDungs.ToListAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string maNguoiDung, int phanQuyen)
        {
            var user = await _context.NguoiDungs.FindAsync(maNguoiDung);
            if (user == null) return NotFound();

            user.PhanQuyen = phanQuyen;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật quyền cho {user.HoVaTen}!";
            return RedirectToAction(nameof(UserIndex));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string maNguoiDung)
        {
            var user = await _context.NguoiDungs.FindAsync(maNguoiDung);
            if (user == null) return NotFound();

            bool coDonHang = await _context.DonHangs.AnyAsync(d => d.MaKhachHang == maNguoiDung);
            if (coDonHang)
            {
                TempData["Error"] = "Tài khoản này đã có đơn hàng, không thể xóa!";
                return RedirectToAction(nameof(UserIndex));
            }

            _context.NguoiDungs.Remove(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa tài khoản!";
            return RedirectToAction(nameof(UserIndex));
        }
    }
}