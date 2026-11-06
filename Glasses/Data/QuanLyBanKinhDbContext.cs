using Microsoft.EntityFrameworkCore;
using Glasses.Models;

namespace Glasses.Data
{
    public class QuanLyBanKinhDbContext : DbContext
    {
        public QuanLyBanKinhDbContext(DbContextOptions options) : base(options) { }

        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<ThuongHieu> ThuongHieus { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<DonHang> ChiTietDonHangs { get; set; }
        public DbSet<NguoiDung> NguoiDungs => Set<NguoiDung>();
        public DbSet<KhachHang> KhachHangs => Set<KhachHang>();
        public DbSet<NhanVien> NhanViens => Set<NhanVien>();
        public DbSet<QuanLy> QuanLies => Set<QuanLy>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasKey(c => new { c.MaDonHang, c.MaBienThe });

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<KhachHang>().HasOne<NguoiDung>()
                .WithOne().HasForeignKey<KhachHang>(k => k.MaKhachHang);
            modelBuilder.Entity<NhanVien>().HasOne<NguoiDung>()
                .WithOne().HasForeignKey<NhanVien>(n => n.MaNhanVien);
            modelBuilder.Entity<QuanLy>().HasOne<NguoiDung>()
                .WithOne().HasForeignKey<QuanLy>(k => k.MaQuanLy);
        }
    }
}
