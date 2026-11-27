using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Glasses.Models
{
    [Table("DanhMuc")]
    public class DanhMuc
    {
        [Key]
        [Column("maDanhMuc")]
        [StringLength(50)]
        public string MaDanhMuc { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [Column("tenDanhMuc")]
        [StringLength(100)]
        public string TenDanhMuc { get; set; } = null;

        // Navigation Property: 1 Danh mục có nhiều Sản phẩm
        public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}
