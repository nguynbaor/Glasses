using System.Security.Claims;
using System.Text.Json;
using Glasses.Data;
using Glasses.Models;
using Glasses.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glasses.Controllers;

[Authorize(Roles = "KhachHang")]
public class CheckoutController : Controller
{
    private readonly QuanLyBanKinhDbContext _db;

    public CheckoutController(QuanLyBanKinhDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var id = User.FindFirstValue(
            ClaimTypes.NameIdentifier)!;

        var user = await _db.NguoiDungs
            .FirstOrDefaultAsync(x => x.MaNguoiDung == id);

        return View(new CheckoutViewModel
        {
            HoTen = user?.HoVaTen ?? "",
            SoDienThoai = user?.SoDienThoai ?? "",
            DiaChi = user?.DiaChi ?? ""
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(
        CheckoutViewModel vm)
    {
        if (vm.PhuongThucThanhToan != "COD" &&
            vm.PhuongThucThanhToan != "Chuyển khoản")
        {
            ModelState.AddModelError(
                nameof(vm.PhuongThucThanhToan),
                "Phương thức thanh toán không hợp lệ.");
        }

        if (!ModelState.IsValid)
            return View("Index", vm);

        List<CartLineInput>? lines;

        try
        {
            lines = JsonSerializer.Deserialize<List<CartLineInput>>(
                vm.CartJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch
        {
            lines = null;
        }

        if (lines == null ||
            lines.Count == 0 ||
            lines.Any(x =>
                string.IsNullOrWhiteSpace(x.Id) ||
                x.Quantity < 1 ||
                x.Quantity > 50))
        {
            ModelState.AddModelError(
                "CartJson",
                "Giỏ hàng không hợp lệ hoặc đang trống.");

            return View("Index", vm);
        }

        lines = lines
            .GroupBy(x => x.Id)
            .Select(g => new CartLineInput
            {
                Id = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        if (lines.Any(x => x.Quantity > 50))
        {
            ModelState.AddModelError(
                "CartJson",
                "Số lượng một sản phẩm không được vượt quá 50.");

            return View("Index", vm);
        }

        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier)!;

        await using var tx =
            await _db.Database.BeginTransactionAsync();

        try
        {
            var variants = await _db.BienThes
                .Include(x => x.SanPham)
                .Where(x => lines.Select(l => l.Id)
                    .Contains(x.MaBienThe))
                .ToListAsync();

            if (variants.Count != lines.Count)
                throw new InvalidOperationException(
                    "Một số sản phẩm không còn tồn tại.");

            decimal total = 0;

            foreach (var line in lines)
            {
                var v = variants.Single(
                    x => x.MaBienThe == line.Id);

                if (v.SoLuongTon < line.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Sản phẩm {v.SanPham?.TenSanPham} " +
                        $"chỉ còn {v.SoLuongTon} sản phẩm.");
                }

                total +=
                    (v.SanPham?.GiaDaGiam ?? 0) *
                    line.Quantity;
            }

            var orderId = "DH" +
                Guid.NewGuid().ToString("N")[..12]
                    .ToUpperInvariant();

            var order = new DonHang
            {
                MaDonHang = orderId,
                MaKhachHang = userId,
                NgayDat = DateTime.Now,
                TrangThai = "Chờ xác nhận",
                TongTien = total,
                HoTenNhan = vm.HoTen.Trim(),
                SoDienThoaiNhan = vm.SoDienThoai.Trim(),
                DiaChiNhan = vm.DiaChi.Trim()
            };

            _db.DonHangs.Add(order);

            await _db.SaveChangesAsync();

            foreach (var line in lines)
            {
                var v = variants.Single(
                    x => x.MaBienThe == line.Id);

                var price = v.SanPham!.GiaDaGiam;

                _db.ChiTietDonHangs.Add(
                    new ChiTietDonHang
                    {
                        MaDonHang = orderId,
                        MaBienThe = v.MaBienThe,
                        DonGia = price,
                        SoLuong = line.Quantity,
                        TongTien = price * line.Quantity
                    });

                v.SoLuongTon -= line.Quantity;
            }

            _db.ThanhToans.Add(new ThanhToan
            {
                MaThanhToan = "TT" +
                    Guid.NewGuid().ToString("N")[..12]
                        .ToUpperInvariant(),

                MaDonHang = orderId,
                HinhThuc = vm.PhuongThucThanhToan,
                SoTien = total,
                NgayThanhToan = null,
                TrangThai = "Chưa thanh toán"
            });

            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            return RedirectToAction(
                nameof(Success),
                new { id = orderId });
        }
        catch (InvalidOperationException ex)
        {
            await tx.RollbackAsync();

            ModelState.AddModelError("", ex.Message);

            return View("Index", vm);
        }
        catch
        {
            await tx.RollbackAsync();

            ModelState.AddModelError(
                "",
                "Không thể tạo đơn hàng. Vui lòng thử lại.");

            return View("Index", vm);
        }
    }

    public async Task<IActionResult> Success(string id)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var order = await _db.DonHangs
            .Include(x => x.ChiTietDonHangs!)
            .ThenInclude(x => x.BienThe)
            .ThenInclude(x => x!.SanPham)
            .FirstOrDefaultAsync(x =>
                x.MaDonHang == id &&
                x.MaKhachHang == userId);

        return order == null
            ? NotFound()
            : View(order);
    }
}
