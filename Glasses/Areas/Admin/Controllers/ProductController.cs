using Glasses.Data;
using Glasses.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Glasses.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly QuanLyBanKinhDbContext _context;

        public ProductController(QuanLyBanKinhDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            // Load danh sách Danh mục và Thương hiệu cho dropdown list
            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
            ViewBag.ThuongHieus = await _context.ThuongHieus.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(SanPham sanPham, IFormFile? HinhAnhFile)
        {
            if (sanPham.Gia > 0)
            {
                sanPham.GiaDaGiam = sanPham.Gia - (sanPham.Gia * sanPham.PhanTramGiam / 100m);
            }

            // Kiểm tra trùng mã sản phẩm
            if (await _context.SanPhams.AnyAsync(s => s.MaSanPham == sanPham.MaSanPham))
            {
                ModelState.AddModelError("MaSanPham", "Mã sản phẩm này đã tồn tại.");
            }

            // 2. Kiểm tra trùng Tên sản phẩm
            if (!string.IsNullOrEmpty(sanPham.TenSanPham) && await _context.SanPhams.AnyAsync(s => s.TenSanPham.Trim().ToLower() == sanPham.TenSanPham.Trim().ToLower()))
            {
                ModelState.AddModelError("TenSanPham", "Tên sản phẩm này đã tồn tại trong hệ thống.");
            }

            ModelState.Remove("DanhMuc");
            ModelState.Remove("ThuongHieu");
            ModelState.Remove("HinhAnh");

            if (ModelState.IsValid)
            {
                if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                {
                    var fileName = Path.GetFileName(HinhAnhFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Admin/images/products", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnhFile.CopyToAsync(stream);
                    }
                    sanPham.HinhAnh = fileName;
                }

                _context.SanPhams.Add(sanPham);
                await _context.SaveChangesAsync();

                return Redirect("/Admin/Product/ProductHienCo");
            }
            return View(sanPham);
        }

        public async Task<IActionResult> ProductHienCo()
        {
            var danhSachSanPham = await _context.SanPhams
                                        .Include(s => s.DanhMuc)
                                        .Include(s => s.ThuongHieu)
                                        .ToListAsync();
            return View(danhSachSanPham);
        }

        [HttpGet]
        public IActionResult AddBrand()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBrand(ThuongHieu thuongHieu)
        {
            // Kiểm tra trùng mã thương hiệu
            if (await _context.ThuongHieus.AnyAsync(t => t.MaThuongHieu == thuongHieu.MaThuongHieu))
            {
                ModelState.AddModelError("MaThuongHieu", "Mã thương hiệu này đã tồn tại.");
            }

            // 2. Kiểm tra trùng Tên thương hiệu
            if (!string.IsNullOrEmpty(thuongHieu.TenThuongHieu) && await _context.ThuongHieus.AnyAsync(t => t.TenThuongHieu.Trim().ToLower() == thuongHieu.TenThuongHieu.Trim().ToLower()))
            {
                ModelState.AddModelError("TenThuongHieu", "Tên thương hiệu này đã tồn tại trong hệ thống.");
            }

            ModelState.Remove("SanPhams");

            if (ModelState.IsValid)
            {
                _context.ThuongHieus.Add(thuongHieu);
                await _context.SaveChangesAsync();

                return Redirect("/Admin/Product/BrandHienCo");
            }

            return View(thuongHieu);
        }

        public async Task<IActionResult> BrandHienCo()
        {
            var danhSachThuongHieu = await _context.ThuongHieus
                                        .Include(s => s.SanPhams)
                                        .ToListAsync();
            return View(danhSachThuongHieu);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(DanhMuc danhMuc)
        {
            // 1. Kiểm tra trùng Mã danh mục
            if (await _context.DanhMucs.AnyAsync(d => d.MaDanhMuc == danhMuc.MaDanhMuc))
            {
                ModelState.AddModelError("MaDanhMuc", "Mã danh mục này đã tồn tại.");
            }

            // 2. Kiểm tra trùng Tên danh mục
            if (!string.IsNullOrEmpty(danhMuc.TenDanhMuc) &&
                await _context.DanhMucs.AnyAsync(d => d.TenDanhMuc.Trim().ToLower() == danhMuc.TenDanhMuc.Trim().ToLower()))
            {
                ModelState.AddModelError("TenDanhMuc", "Tên danh mục này đã tồn tại trong hệ thống.");
            }

            ModelState.Remove("SanPhams");

            if (ModelState.IsValid)
            {
                _context.DanhMucs.Add(danhMuc);
                await _context.SaveChangesAsync();

                return Redirect("/Admin/Product/CategoryHienCo");
            }

            return View(danhMuc);
        }

        public async Task<IActionResult> CategoryHienCo()
        {
            var danhSachDanhMuc = await _context.DanhMucs
                                        .Include(s => s.SanPhams)
                                        .ToListAsync();
            return View(danhSachDanhMuc);
        }

        public async Task<IActionResult> OrderHienCo(string search)
        {
            var query = _context.DonHangs
                                .Include(d => d.ChiTietDonHangs)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(d => d.MaDonHang.ToLower().Contains(search)
                                      || (d.MaKhachHang != null && d.MaKhachHang.ToLower().Contains(search)));
            }

            var danhSachDonHang = await query.OrderByDescending(d => d.NgayDat).ToListAsync();

            return View(danhSachDonHang);
        }

        public IActionResult OrderTracking()
        {
            return View();
        }
    }
}