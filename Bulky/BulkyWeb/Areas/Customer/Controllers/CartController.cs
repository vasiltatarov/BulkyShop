namespace BulkyWeb.Areas.Customer.Controllers;

using Bulky.Models;

[Area(SD.Role_Customer)]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public CartController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    public IActionResult Index()
    {
        var userClaims = (ClaimsIdentity)User.Identity;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

        this.ShoppingCartVM = new()
        {
            ShoppingCartList = this.unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId, "Product"),
            OrderHeader = new()
        };

        this.CalculateTotalPrice();

        return View(this.ShoppingCartVM);
    }

    public IActionResult Summary()
    {
        var userClaims = (ClaimsIdentity)User.Identity;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

        this.ShoppingCartVM = new()
        {
            ShoppingCartList = this.unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId, "Product"),
            OrderHeader = new()
        };

        var user = this.unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

        this.ShoppingCartVM.OrderHeader.User = user;
        this.ShoppingCartVM.OrderHeader.Name = user.Name;
        this.ShoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
        this.ShoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
        this.ShoppingCartVM.OrderHeader.City = user.City;
        this.ShoppingCartVM.OrderHeader.State = user.State;
        this.ShoppingCartVM.OrderHeader.PostalCode = user.PostalCode;

        this.CalculateTotalPrice();

        return View(this.ShoppingCartVM);
    }

    [HttpPost]
    [ActionName("Summary")]
    public IActionResult SummaryPost()
    {
        var userClaims = (ClaimsIdentity)User.Identity;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

        this.ShoppingCartVM.ShoppingCartList = this.unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId, "Product");
        this.ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
        this.ShoppingCartVM.OrderHeader.UserId = userId;

        var user = this.unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

        this.CalculateTotalPrice();

        if (user.CompanyId.GetValueOrDefault() != 0)
        {
            //it is a company user
            this.ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            this.ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
        }
        else
        {
            //it is a regular customer 
            this.ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            this.ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
        }

        this.unitOfWork.OrderHeaderRepository.Add(this.ShoppingCartVM.OrderHeader);
        this.unitOfWork.Save();

        foreach (var cart in this.ShoppingCartVM.ShoppingCartList)
        {
            var orderDetail = new OrderDetail
            {
                OrderHeaderId = this.ShoppingCartVM.OrderHeader.Id,
                ProductId = cart.ProductId,
                Count = cart.Count,
                Price = cart.Price
            };

            this.unitOfWork.OrderDetailRepository.Add(orderDetail);
            this.unitOfWork.Save();
        }

        if (user.CompanyId.GetValueOrDefault() == 0)
        {
            //it's a regular customer account and we need to capture payment
            //stripe logic
        }

        return RedirectToAction("OrderConfirmation", new { id = this.ShoppingCartVM.OrderHeader.Id });
    }

    public IActionResult OrderConfirmation(int id)
    {
        return View(id);
    }

    public IActionResult Plus(int cartId)
    {
        var cartFromDb = this.unitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId);
        cartFromDb.Count++;

        this.unitOfWork.ShoppingCartRepository.Update(cartFromDb);
        this.unitOfWork.Save();

        return RedirectToAction("Index");
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDb = this.unitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId);

        if (cartFromDb.Count > 1)
        {
            cartFromDb.Count--;
            this.unitOfWork.ShoppingCartRepository.Update(cartFromDb);
            this.unitOfWork.Save();
        }

        return RedirectToAction("Index");
    }
    public IActionResult Remove(int cartId)
    {
        var cartFromDb = this.unitOfWork.ShoppingCartRepository.Get(x => x.Id == cartId);

        this.unitOfWork.ShoppingCartRepository.Remove(cartFromDb);
        this.unitOfWork.Save();

        return RedirectToAction("Index");
    }

    private void CalculateTotalPrice()
    {
        double total = 0;

        foreach (var cart in this.ShoppingCartVM.ShoppingCartList)
        {
            double price = this.GetPriceBasedOnQuantity(cart);

            cart.Price = price;
            total += price * cart.Count;
        }

        this.ShoppingCartVM.OrderHeader.OrderTotal = total;
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }
        else
        {
            if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
    }
}
