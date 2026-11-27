using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Glasses.Models
{
    [Table("ChiTietGioHang")]
    [PrimaryKey(nameof(MaGioHang), nameof(MaBienThe))]
    public class ChiTietGioHang
    {
        [Column("maGioHang")]
        [StringLength(50)]
        public string MaGioHang { get; set; } = string.Empty;

        [Column("maBienThe")]
        [StringLength(50)]
        public string MaBienThe { get; set; } = string.Empty;

        [Required]
        [Column("soLuong")]
        public int SoLuong { get; set; }

        [ForeignKey("MaGioHang")]
        public virtual GioHang? GioHang { get; set; }

        [ForeignKey("MaBienThe")]
        public virtual BienThe? BienThe { get; set; }
    }
}