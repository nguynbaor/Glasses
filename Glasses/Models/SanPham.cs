using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glasses.Models
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        [Column("maSanPham")]
        [StringLength(50)]
        public string MaSanPham { get; set; } = null!;

        [Column("maDanhMuc")]
        [StringLength(50)]
        public string? MaDanhMuc { get; set; }

        [Column("maThuongHieu")]
        [StringLength(50)]
        public string? MaThuongHieu { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [Column("tenSanPham")]
        [StringLength(255)]
        public string TenSanPham { get; set; } = null!;

        [Column("thongTin")]
        public string? ThongTin { get; set; }

        [Required]
        [Column("gia", TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        [Column("phanTramGiam")]
        public int PhanTramGiam { get; set; }

        [Required]
        [Column("giaDaGiam", TypeName = "decimal(18,2)")]
        public decimal GiaDaGiam { get; set; }

        [Column("chatLieu")]
        [StringLength(100)]
        public string? ChatLieu { get; set; }

        [Column("HinhAnh")]
        public string? HinhAnh { get; set; }

        // Khóa ngoại liên kết bảng DanhMuc
        [ForeignKey("MaDanhMuc")]
        public virtual DanhMuc? DanhMuc { get; set; }

        // Khóa ngoại liên kết bảng ThuongHieu
        [ForeignKey("MaThuongHieu")]
        public virtual ThuongHieu? ThuongHieu { get; set; }
    }
}
