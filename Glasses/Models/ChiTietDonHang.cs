using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glasses.Models
{
    [Table("ChiTietDonHang")]
    public class ChiTietDonHang
    {
        [Column("maDonHang")]
        [StringLength(50)]
        public string MaDonHang { get; set; } = string.Empty;

        [Column("maBienThe")]
        [StringLength(50)]
        public string MaBienThe { get; set; } = string.Empty;

        [Column("donGia", TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [Column("soLuong")]
        public int SoLuong { get; set; }

        [Column("tongTien", TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [ForeignKey("MaDonHang")]
        public virtual DonHang? DonHang { get; set; }
    }
}