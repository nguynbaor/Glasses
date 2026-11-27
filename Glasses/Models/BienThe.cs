using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glasses.Models
{
    [Table("BienThe")]
    public class BienThe
    {
        [Key]
        [Column("maBienThe")]
        [StringLength(50)]
        public string MaBienThe { get; set; } = string.Empty;

        [Required]
        [Column("maSanPham")]
        [StringLength(50)]
        public string MaSanPham { get; set; } = string.Empty;

        [Column("mauSac")]
        [StringLength(50)]
        public string? MauSac { get; set; }

        [Column("hinhAnh")]
        [StringLength(255)]
        public string? HinhAnh { get; set; }

        [Column("soLuongTon")]
        public int SoLuongTon { get; set; } = 0;

        [ForeignKey("MaSanPham")]
        public virtual SanPham? SanPham { get; set; }

        public virtual ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public virtual ICollection<ChiTietGioHang>? ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
    }
}