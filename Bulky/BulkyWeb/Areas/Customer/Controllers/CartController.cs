namespace BulkyWeb.Areas.Customer.Controllers;

using Stripe.Checkout;

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
    public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

    public IActionResult Index()
    {
        var userClaims = (ClaimsIdentity)User.Identity;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

        this.ShoppingCartViewModel = new()
        {
            ShoppingCartList = this.unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId, "Product"),
            OrderHeader = new()
        };

        this.CalculateTotalPrice();

        return View(this.ShoppingCartViewModel);
    }

    public IActionResult Summary()
    {
        var userClaims = (ClaimsIdentity)User.Identity;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

        this.ShoppingCartViewModel = new()
        {
            ShoppingCartList = this.unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId, "Product"),
            OrderHeader = new()
        };

        var user = this.unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

        this.ShoppingCartViewModel.OrderHeader.User = user;
        this.ShoppingCartViewModel.OrderHeader.Name = user.Name;
        this.ShoppingCartViewModel.OrderHeader.PhoneNumber = user.PhoneNumber;
        this.ShoppingCartViewModel.OrderHeader.StreetAddress = user.StreetAddress;
        this.ShoppingCartViewModel.OrderHeader.City = user.City;
        this.ShoppingCartViewModel.OrderHeader.State = user.State;
        this.ShoppingCartViewModel.OrderHeader.PostalCode = user.PostalCode;

        this.CalculateTotalPrice();

        return View(this.ShoppingCartViewModel);
    }

    [HttpPost]
    [ActionName("Summary")]
    public IActionResult SummaryPost()
    {
        var userClaims = (ClaimsIdentity)User.Identity;
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier).Value;

        this.ShoppingCartViewModel.ShoppingCartList = this.unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == userId, "Product");
        this.ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
        this.ShoppingCartViewModel.OrderHeader.UserId = userId;

        var user = this.unitOfWork.ApplicationUserRepository.Get(u => u.Id == userId);

        this.CalculateTotalPrice();

        if (user.CompanyId.GetValueOrDefault() != 0)
        {
            //it is a company user
            this.ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            this.ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusApproved;
        }
        else
        {
            //it is a regular customer 
            this.ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            this.ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusPending;
        }

        this.unitOfWork.OrderHeaderRepository.Add(this.ShoppingCartViewModel.OrderHeader);
        this.unitOfWork.Save();

        foreach (var cart in this.ShoppingCartViewModel.ShoppingCartList)
        {
            var orderDetail = new OrderDetail
            {
                OrderHeaderId = this.ShoppingCartViewModel.OrderHeader.Id,
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

            var domain = "http://localhost:5237/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{domain}Customer/Cart/OrderConfirmation?id={this.ShoppingCartViewModel.OrderHeader.Id}",
                CancelUrl = $"{domain}Customer/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var cart in this.ShoppingCartViewModel.ShoppingCartList)
            {
                var lineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)cart.Price * 100, // 20.50 * 100 => 2050
                        Currency = "BGN",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Product.Title
                        }
                    },
                    Quantity = cart.Count
                };

                options.LineItems.Add(lineItem);
            }

            var service = new SessionService();
            var session = service.Create(options);

            this.unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(this.ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
            this.unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        return RedirectToAction("OrderConfirmation", new { id = this.ShoppingCartViewModel.OrderHeader.Id });
    }

    public IActionResult OrderConfirmation(int id)
    {
        var orderHeader = this.unitOfWork.OrderHeaderRepository.Get(x => x.Id == id, "User");

        if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
        {
            //this is an order by customer

            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                this.unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                this.unitOfWork.OrderHeaderRepository.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                this.unitOfWork.Save();
            }
        }

        var shoppingCarts = this.unitOfWork.ShoppingCartRepository.GetAll(x => x.UserId == orderHeader.UserId);
        this.unitOfWork.ShoppingCartRepository.RemoveRange(shoppingCarts);
        this.unitOfWork.Save();

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

        foreach (var cart in this.ShoppingCartViewModel.ShoppingCartList)
        {
            double price = this.GetPriceBasedOnQuantity(cart);

            cart.Price = price;
            total += price * cart.Count;
        }

        this.ShoppingCartViewModel.OrderHeader.OrderTotal = total;
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
