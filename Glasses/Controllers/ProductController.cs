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

        public async Task<IActionResult> Index(string categoryId, string? mainCategory, decimal? minPrice, decimal? maxPrice, string sortBy = "default")
        {
            var query = _context.SanPhams.Include(s => s.BienThes).AsQueryable();

            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(s => s.MaDanhMuc != null && s.MaDanhMuc.Trim().ToLower() == categoryId.Trim().ToLower());
            }

            if (minPrice.HasValue && minPrice.Value > 0)
            {
                query = query.Where(s => s.GiaDaGiam >= minPrice.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(s => s.GiaDaGiam <= maxPrice.Value);
            }

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(s => s.GiaDaGiam),
                "price_desc" => query.OrderByDescending(s => s.GiaDaGiam),
                _ => query.OrderByDescending(s => s.MaSanPham)
            };

            var products = await query.ToListAsync();
            var category = await _context.DanhMucs.FirstOrDefaultAsync(d => d.MaDanhMuc == categoryId);

            var viewModel = new ProductListViewModel
            {
                Products = products,
                ActiveCategoryId = categoryId ?? string.Empty,
                CurrentCategoryName = category != null ? category.TenDanhMuc : "Tất cả sản phẩm",
                MainCategoryName = !string.IsNullOrEmpty(mainCategory) ? mainCategory : (category != null ? category.TenDanhMuc : "Danh mục"),
                TotalCount = products.Count,
                SelectedSort = sortBy,
                MinPrice = minPrice ?? 0,
                MaxPrice = maxPrice ?? 0
            };

            return View(viewModel);
        }
    }
}
