namespace Glasses.Models.ViewModels
{
    public class CategorySidebarViewModel
    {
        public List<DanhMuc> Categories { get; set; } = new List<DanhMuc>();
        public string ActiveCategoryId { get; set; } = string.Empty;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 10000000;
        public decimal CurrentMinPrice { get; set; }
        public decimal CurrentMaxPrice { get; set; }
    }

    public class ProductListViewModel
    {
        public List<SanPham> Products { get; set; } = new List<SanPham>();
        public string CurrentCategoryName { get; set; } = "Tất cả sản phẩm";
        public string MainCategoryName { get; set; } = "Danh mục";
        public string ActiveCategoryId { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public string SelectedSort { get; set; } = "default";
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
