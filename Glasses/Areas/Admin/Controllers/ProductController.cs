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
        public async Task<IActionResult> Details(string id)
        {
            var item = await _context.SanPhams
                .Include(x => x.DanhMuc)
                .Include(x => x.ThuongHieu)
                .Include(x => x.BienThes)
                .FirstOrDefaultAsync(x => x.MaSanPham == id);
        
            return item == null ? NotFound() : View("Details", item);
        }
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            // Load danh sách Danh mục và Thương hiệu cho dropdown list
            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
            ViewBag.ThuongHieus = await _context.ThuongHieus.ToListAsync();

            return View();
        }
        [HttpGet]
public async Task<IActionResult> Edit(string id)
{
    var item = await _context.SanPhams
        .FirstOrDefaultAsync(x => x.MaSanPham == id);

    if (item == null)
        return NotFound();

    await LoadProductLists();

    return View(item);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(
    string id,
    SanPham model,
    IFormFile? HinhAnhFile)
{
    if (id != model.MaSanPham)
        return BadRequest();

    ModelState.Remove("DanhMuc");
    ModelState.Remove("ThuongHieu");
    ModelState.Remove("HinhAnh");
    ModelState.Remove("BienThes");

    if (model.Gia < 0 ||
        model.PhanTramGiam < 0 ||
        model.PhanTramGiam > 100)
    {
        ModelState.AddModelError(
            "Gia",
            "Giá hoặc phần trăm giảm không hợp lệ.");
    }

    if (!ModelState.IsValid)
    {
        await LoadProductLists();
        return View(model);
    }

    var item = await _context.SanPhams
        .FirstOrDefaultAsync(x => x.MaSanPham == id);

    if (item == null)
        return NotFound();

    item.TenSanPham = model.TenSanPham;
    item.MaDanhMuc = model.MaDanhMuc;
    item.MaThuongHieu = model.MaThuongHieu;
    item.ThongTin = model.ThongTin;
    item.Gia = model.Gia;
    item.PhanTramGiam = model.PhanTramGiam;

    item.GiaDaGiam =
        model.Gia * (100 - model.PhanTramGiam) / 100m;

    item.ChatLieu = model.ChatLieu;

    if (HinhAnhFile?.Length > 0)
        item.HinhAnh = await SaveProductImage(HinhAnhFile);

    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(ProductHienCo));
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(string id)
{
    var item = await _context.SanPhams
        .Include(x => x.BienThes)
        .FirstOrDefaultAsync(x => x.MaSanPham == id);

    if (item == null)
        return NotFound();

    if (item.BienThes.Any(v =>
        _context.ChiTietDonHangs.Any(
            c => c.MaBienThe == v.MaBienThe)))
    {
        TempData["Error"] =
            "Không thể xóa sản phẩm đã phát sinh đơn hàng.";

        return RedirectToAction(nameof(ProductHienCo));
    }

    _context.BienThes.RemoveRange(item.BienThes);
    _context.SanPhams.Remove(item);

    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(ProductHienCo));
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
