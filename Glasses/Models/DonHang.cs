using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glasses.Models
{
    [Table("DonHang")]
    public class DonHang
    {
        [Key]
        [Column("maDonHang")]
        [StringLength(50)]
        public string MaDonHang { get; set; } = string.Empty;

        [Column("maKhachHang")]
        [StringLength(50)]
        public string? MaKhachHang { get; set; }

        [Column("ngayDat")]
        public DateTime? NgayDat { get; set; }

        [Column("trangThai")]
        [StringLength(50)]
        public string? TrangThai { get; set; }

        [Column("tongTien", TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        public virtual ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}