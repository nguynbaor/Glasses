using System.ComponentModel.DataAnnotations;

namespace Glasses.Models.ViewModels;

public class CheckoutViewModel
{
    [Required, StringLength(100)]
    public string HoTen { get; set; } = "";

    [Required, Phone, StringLength(15)]
    public string SoDienThoai { get; set; } = "";

    [Required, StringLength(255)]
    public string DiaChi { get; set; } = "";

    [Required]
    public string PhuongThucThanhToan { get; set; } = "COD";

    [Required]
    public string CartJson { get; set; } = "[]";
}

public class CartLineInput
{
    public string Id { get; set; } = "";

    public int Quantity { get; set; }
}
