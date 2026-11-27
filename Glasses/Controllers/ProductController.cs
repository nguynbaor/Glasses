using Glasses.Data;
using Glasses.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glasses.Controllers
{
    public class ProductController : Controller
    {
        private readonly QuanLyBanKinhDbContext _context;

        public ProductController(QuanLyBanKinhDbContext context)
        {
            _context = context;
        }

        // Tìm kiếm theo tên sản phẩm hoặc tên thương hiệu, đồng thời giữ bộ lọc hiện có.
        public async Task<IActionResult> Index(
            string? categoryId,
            string? mainCategory,
            decimal? minPrice,
            decimal? maxPrice,
            string sortBy = "default",
            string? q = null)
        {
            var query = _context.SanPhams
                .AsNoTracking()
                .Include(s => s.BienThes)
                .Include(s => s.ThuongHieu)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(s =>
                    s.TenSanPham.Contains(q) ||
                    (s.ThuongHieu != null && s.ThuongHieu.TenThuongHieu.Contains(q)));
            }

            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                var normalizedCategoryId = categoryId.Trim();
                query = query.Where(s => s.MaDanhMuc != null && s.MaDanhMuc.Trim() == normalizedCategoryId);
            }

            if (minPrice.HasValue && minPrice.Value > 0)
                query = query.Where(s => s.GiaDaGiam >= minPrice.Value);

            if (maxPrice.HasValue && maxPrice.Value > 0)
                query = query.Where(s => s.GiaDaGiam <= maxPrice.Value);

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(s => s.GiaDaGiam),
                "price_desc" => query.OrderByDescending(s => s.GiaDaGiam),
                _ => query.OrderByDescending(s => s.MaSanPham)
            };

            var products = await query.ToListAsync();
            var category = await _context.DanhMucs
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDanhMuc == categoryId);

            var viewModel = new ProductListViewModel
            {
                Products = products,
                ActiveCategoryId = categoryId ?? string.Empty,
                CurrentCategoryName = !string.IsNullOrWhiteSpace(q)
                    ? $"Kết quả tìm kiếm: {q}"
                    : category?.TenDanhMuc ?? "Tất cả sản phẩm",
                MainCategoryName = !string.IsNullOrEmpty(mainCategory)
                    ? mainCategory
                    : category?.TenDanhMuc ?? "Danh mục",
                TotalCount = products.Count,
                SelectedSort = sortBy,
                MinPrice = minPrice ?? 0,
                MaxPrice = maxPrice ?? 0,
                SearchTerm = q ?? string.Empty
            };

            return View(viewModel);
        }

        // API gợi ý nhanh khi người dùng đang nhập từ khóa.
        [HttpGet]
        public async Task<IActionResult> Suggestions(string? term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 2)
                return Json(Array.Empty<object>());

            var keyword = term.Trim();
            var suggestions = await _context.SanPhams
                .AsNoTracking()
                .Where(s => s.TenSanPham.Contains(keyword) ||
                    (s.ThuongHieu != null && s.ThuongHieu.TenThuongHieu.Contains(keyword)))
                .OrderBy(s => s.TenSanPham)
                .Select(s => new
                {
                    name = s.TenSanPham,
                    brand = s.ThuongHieu != null ? s.ThuongHieu.TenThuongHieu : string.Empty
                })
                .Distinct()
                .Take(8)
                .ToListAsync();

            return Json(suggestions);
        }
    }
}
