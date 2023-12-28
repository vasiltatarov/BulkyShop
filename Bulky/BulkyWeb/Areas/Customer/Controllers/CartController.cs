namespace BulkyWeb.Areas.Customer.Controllers;

[Area(SD.Role_Customer)]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public CartController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var userClaims = (ClaimsIdentity)User.Identity;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

        var viewModel = new ShoppingCartVM
        {
            ShoppingCartList = this.unitOfWork.ShopingCartRepository.GetAll(x => x.UserId == userId, "Product")
        };

        this.CalculatePrices(viewModel);

        return View(viewModel);
    }

    public IActionResult Plus(int cartId)
    {
        var cartFromDb = this.unitOfWork.ShopingCartRepository.Get(x => x.Id == cartId);
        cartFromDb.Count++;

        this.unitOfWork.ShopingCartRepository.Update(cartFromDb);
        this.unitOfWork.Save();

        return RedirectToAction("Index");
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDb = this.unitOfWork.ShopingCartRepository.Get(x => x.Id == cartId);

        if (cartFromDb.Count > 1)
        {
            cartFromDb.Count--;
            this.unitOfWork.ShopingCartRepository.Update(cartFromDb);
            this.unitOfWork.Save();
        }

        return RedirectToAction("Index");
    }
    public IActionResult Remove(int cartId)
    {
        var cartFromDb = this.unitOfWork.ShopingCartRepository.Get(x => x.Id == cartId);

        this.unitOfWork.ShopingCartRepository.Remove(cartFromDb);
        this.unitOfWork.Save();

        return RedirectToAction("Index");
    }

    private void CalculatePrices(ShoppingCartVM cartViewModel)
    {
        double total = 0;

        foreach (var product in cartViewModel.ShoppingCartList)
        {
            double price = 0;

            if (product.Count <= 50)
            {
                price = product.Product.Price;
            }
            else if (product.Count <= 100)
            {
                price = product.Product.Price50;
            }
            else
            {
                price = product.Product.Price100;
            }

            product.Price = price;
            total += price * product.Count;
        }

        cartViewModel.OrderTotal = total;
    }
}
