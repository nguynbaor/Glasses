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
        // 1. SỬA SẢN PHẨM
        [HttpGet]
        public async Task<IActionResult> EditProduct(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();

            ViewBag.DanhMucs = new SelectList(_context.DanhMucs, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            ViewBag.ThuongHieus = new SelectList(_context.ThuongHieus, "MaThuongHieu", "TenThuongHieu", sanPham.MaThuongHieu);
            return View(sanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(string id, SanPham model, IFormFile? HinhAnhFile)
        {
            if (id != model.MaSanPham) return BadRequest();

            var existing = await _context.SanPhams.FindAsync(id);
            if (existing == null) return NotFound();

            existing.TenSanPham = model.TenSanPham;
            existing.ThongTin = model.ThongTin;
            existing.ChatLieu = model.ChatLieu;
            existing.MaDanhMuc = model.MaDanhMuc;
            existing.MaThuongHieu = model.MaThuongHieu;
            existing.Gia = model.Gia;
            existing.PhanTramGiam = Math.Clamp(model.PhanTramGiam, 0, 100);
            // Tự động tính lại Giá đã giảm
            existing.GiaDaGiam = existing.Gia * (100 - existing.PhanTramGiam) / 100m;

            if (HinhAnhFile != null && HinhAnhFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(HinhAnhFile.FileName)}";
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "images", "products");
                Directory.CreateDirectory(uploadDir);

                using var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create);
                await HinhAnhFile.CopyToAsync(stream);
                existing.HinhAnh = fileName;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(ProductHienCo));
        }

        // 2. XÓA SẢN PHẨM (An toàn khóa ngoại)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.BienThes)
                .FirstOrDefaultAsync(s => s.MaSanPham == id);

            if (sanPham == null) return NotFound();

            var maBienThes = sanPham.BienThes.Select(b => b.MaBienThe).ToList();
            bool daCoDonHang = await _context.ChiTietDonHangs.AnyAsync(ct => maBienThes.Contains(ct.MaBienThe));

            if (daCoDonHang)
            {
                TempData["Error"] = "Sản phẩm đã phát sinh đơn hàng, không thể xóa! Vui lòng chỉnh tồn kho về 0.";
                return RedirectToAction(nameof(ProductHienCo));
            }

            if (sanPham.BienThes.Any())
            {
                // Xóa trong giỏ hàng nếu có trước khi xóa biến thể
                var chiTietGioHangs = _context.ChiTietGioHangs.Where(ct => maBienThes.Contains(ct.MaBienThe));
                _context.ChiTietGioHangs.RemoveRange(chiTietGioHangs);
                _context.BienThes.RemoveRange(sanPham.BienThes);
            }

            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa sản phẩm!";
            return RedirectToAction(nameof(ProductHienCo));
        }

        // 3. QUẢN LÝ BIẾN THỂ KÍNH (Màu sắc, ảnh riêng theo màu, tồn kho)
        [HttpGet]
        public async Task<IActionResult> Variants(string maSanPham)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.BienThes)
                .FirstOrDefaultAsync(s => s.MaSanPham == maSanPham);

            if (sanPham == null) return NotFound();
            return View(sanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVariant(string MaSanPham, string MauSac, int SoLuongTon, IFormFile? HinhAnhBienThe)
        {
            var bienThe = new BienThe
            {
                MaBienThe = "BT" + DateTime.Now.ToString("yyMMddHHmmssfff"),
                MaSanPham = MaSanPham,
                MauSac = MauSac,
                SoLuongTon = Math.Max(0, SoLuongTon)
            };

            if (HinhAnhBienThe != null && HinhAnhBienThe.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(HinhAnhBienThe.FileName)}";
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "images", "products");
                Directory.CreateDirectory(uploadDir);

                using var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create);
                await HinhAnhBienThe.CopyToAsync(stream);
                bienThe.HinhAnh = fileName;
            }

            _context.BienThes.Add(bienThe);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã thêm biến thể mới!";
            return RedirectToAction(nameof(Variants), new { maSanPham = MaSanPham });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVariantStock(string maBienThe, int soLuongTon)
        {
            var bt = await _context.BienThes.FindAsync(maBienThe);
            if (bt == null) return NotFound();

            bt.SoLuongTon = Math.Max(0, soLuongTon);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật số lượng tồn!";
            return RedirectToAction(nameof(Variants), new { maSanPham = bt.MaSanPham });
        }
        // GET: Admin/Product/OrderDetail?id=DH001
        [HttpGet]
        public async Task<IActionResult> OrderDetail(string id)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null) return NotFound();

            // Lấy thông tin khách hàng từ bảng NguoiDung (vì MaKhachHang = MaNguoiDung)
            var khachHang = await _context.NguoiDungs.FindAsync(donHang.MaKhachHang);

            // Join ChiTietDonHang -> BienThe -> SanPham để lấy tên kính & màu sắc
            var chiTiets = await (from ct in _context.ChiTietDonHangs
                                  join bt in _context.BienThes on ct.MaBienThe equals bt.MaBienThe
                                  join sp in _context.SanPhams on bt.MaSanPham equals sp.MaSanPham
                                  where ct.MaDonHang == id
                                  select new
                                  {
                                      ct.MaBienThe,
                                      sp.TenSanPham,
                                      bt.MauSac,
                                      HinhAnh = bt.HinhAnh ?? sp.HinhAnh,
                                      ct.DonGia,
                                      ct.SoLuong,
                                      ct.TongTien
                                  }).ToListAsync();

            ViewBag.KhachHang = khachHang;
            ViewBag.ChiTiets = chiTiets;
            return View(donHang);
        }

        // POST: Admin/Product/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(string maDonHang, string trangThaiMoi)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.MaDonHang == maDonHang);

            if (donHang == null) return NotFound();

            // Nếu chuyển sang "Đã hủy" (và trước đó chưa hủy), hoàn lại tồn kho cho từng biến thể
            if (trangThaiMoi == "Đã hủy" && donHang.TrangThai != "Đã hủy" && donHang.ChiTietDonHangs != null)
            {
                foreach (var item in donHang.ChiTietDonHangs)
                {
                    var bt = await _context.BienThes.FindAsync(item.MaBienThe);
                    if (bt != null) bt.SoLuongTon += item.SoLuong;
                }
            }

            donHang.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật đơn {maDonHang} sang trạng thái: {trangThaiMoi}";
            return RedirectToAction(nameof(OrderDetail), new { id = maDonHang });
        }
        // ==========================================
        // SỬA & XÓA DANH MỤC (DanhMuc)
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> EditCategory(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null) return NotFound();

            return View(danhMuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(string id, DanhMuc model)
        {
            if (id != model.MaDanhMuc) return BadRequest();

            var existing = await _context.DanhMucs.FindAsync(id);
            if (existing == null) return NotFound();

            if (string.IsNullOrWhiteSpace(model.TenDanhMuc))
            {
                TempData["Error"] = "Tên danh mục không được để trống!";
                return RedirectToAction(nameof(CategoryHienCo));
            }

            existing.TenDanhMuc = model.TenDanhMuc.Trim();
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật danh mục thành công!";
            return RedirectToAction(nameof(CategoryHienCo));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null) return NotFound();

            // Kiểm tra xem có sản phẩm nào đang thuộc danh mục này không
            bool dangSuDung = await _context.SanPhams.AnyAsync(sp => sp.MaDanhMuc == id);
            if (dangSuDung)
            {
                TempData["Error"] = $"Không thể xóa danh mục '{danhMuc.TenDanhMuc}' vì đang có sản phẩm thuộc danh mục này!";
                return RedirectToAction(nameof(CategoryHienCo));
            }

            _context.DanhMucs.Remove(danhMuc);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa danh mục thành công!";
            return RedirectToAction(nameof(CategoryHienCo));
        }

        // ==========================================
        // SỬA & XÓA THƯƠNG HIỆU (ThuongHieu)
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> EditBrand(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var thuongHieu = await _context.ThuongHieus.FindAsync(id);
            if (thuongHieu == null) return NotFound();

            return View(thuongHieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBrand(string id, ThuongHieu model)
        {
            if (id != model.MaThuongHieu) return BadRequest();

            var existing = await _context.ThuongHieus.FindAsync(id);
            if (existing == null) return NotFound();

            if (string.IsNullOrWhiteSpace(model.TenThuongHieu))
            {
                TempData["Error"] = "Tên thương hiệu không được để trống!";
                return RedirectToAction(nameof(BrandHienCo));
            }

            existing.TenThuongHieu = model.TenThuongHieu.Trim();
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thương hiệu thành công!";
            return RedirectToAction(nameof(BrandHienCo));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBrand(string id)
        {
            var thuongHieu = await _context.ThuongHieus.FindAsync(id);
            if (thuongHieu == null) return NotFound();

            // Kiểm tra xem có sản phẩm nào đang thuộc thương hiệu này không
            bool dangSuDung = await _context.SanPhams.AnyAsync(sp => sp.MaThuongHieu == id);
            if (dangSuDung)
            {
                TempData["Error"] = $"Không thể xóa thương hiệu '{thuongHieu.TenThuongHieu}' vì đang có sản phẩm thuộc thương hiệu này!";
                return RedirectToAction(nameof(BrandHienCo));
            }

            _context.ThuongHieus.Remove(thuongHieu);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa thương hiệu thành công!";
            return RedirectToAction(nameof(BrandHienCo));
        }
    }
}