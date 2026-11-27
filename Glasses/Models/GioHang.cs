using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glasses.Models
{
    [Table("GioHang")]
    public class GioHang
    {
        [Key]
        [Column("maGioHang")]
        [StringLength(50)]
        public string MaGioHang { get; set; } = string.Empty;

        [Required]
        [Column("maKhachHang")]
        [StringLength(50)]
        public string MaKhachHang { get; set; } = string.Empty;

        [Column("tongTien", TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; } = 0;

        [ForeignKey("MaKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        public virtual ICollection<ChiTietGioHang>? ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
    }
}