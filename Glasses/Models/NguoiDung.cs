using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glasses.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        [Column("maNguoiDung")]
        [StringLength(50)]
        public string MaNguoiDung { get; set; } = "";

        [Column("matKhau")]
        [StringLength(255)]
        public string MatKhau { get; set; } = "";

        [Column("hoVaTen")]
        [StringLength(100)]
        public string HoVaTen { get; set; } = "";

        [Column("soDienThoai")]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = "";

        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; } = "";

        [Column("diaChi")]
        [StringLength(255)]
        public string? DiaChi { get; set; }

        [Column("phanQuyen")]
        public int PhanQuyen { get; set; }
    }

    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        [Column("maKhachHang")]
        [StringLength(50)]
        public string MaKhachHang { get; set; } = "";
    }

    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        [Column("maNhanVien")]
        [StringLength(50)]
        public string MaNhanVien { get; set; } = "";
    }

    [Table("QuanLy")]
    public class QuanLy
    {
        [Key]
        [Column("maQuanLy")]
        [StringLength(50)]
        public string MaQuanLy { get; set; } = "";
    }
}
