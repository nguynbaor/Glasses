using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Glasses.Models
{
    [Table("ThuongHieu")]
    public class ThuongHieu
    {
        [Key]
        [Column("maThuongHieu")]
        [StringLength(50)]
        public string MaThuongHieu { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên thương hiệu")]
        [Column("tenThuongHieu")]
        [StringLength(100)]
        public string TenThuongHieu { get; set; } = null!;

        // Navigation Property: 1 Thương hiệu có nhiều Sản phẩm
        public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}
