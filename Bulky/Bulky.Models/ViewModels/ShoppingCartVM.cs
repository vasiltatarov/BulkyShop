namespace Bulky.Models.ViewModels;

public class ShoppingCartVM
{
    public IEnumerable<ShopingCart> ShoppingCartList { get; set; }

    public double OrderTotal { get; set; }
}
