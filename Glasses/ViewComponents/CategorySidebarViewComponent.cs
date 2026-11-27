using Glasses.Data;
using Glasses.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glasses.ViewComponents
{
    public class CategorySidebarViewComponent : ViewComponent
    {
        private readonly QuanLyBanKinhDbContext _context;

        public CategorySidebarViewComponent(QuanLyBanKinhDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string activeCategoryId, decimal? minPrice, decimal? maxPrice)
        {
            var categories = await _context.DanhMucs.ToListAsync();
            var model = new CategorySidebarViewModel
            {
                Categories = categories,
                ActiveCategoryId = activeCategoryId,
                MinPrice = minPrice ?? 0,
                MaxPrice = maxPrice ?? 0
            };
            return View(model);
        }
    }
}
