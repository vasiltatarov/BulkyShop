namespace BulkyWeb.Areas.Customer.Controllers;

[Area(SD.Role_Customer)]
public class HomeController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var products = this.unitOfWork.ProductRepository.GetAll();

        return View(products);
    }

    public IActionResult Details(int productId)
    {
        var product = this.unitOfWork.ProductRepository.Get(x => x.Id == productId, "Category");

        if (product == null)
        {
            return NotFound();
        }

        var cart = new ShoppingCart
        {
            Product = product,
            ProductId = productId,
            Count = 1
        };

        return View(cart);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart cart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        cart.UserId = userId;
        var cartFromDb = this.unitOfWork.ShoppingCartRepository.Get(x => x.ProductId == cart.ProductId && x.UserId == userId);

        if (cartFromDb != null)
        {
            //Edit Cart Quantity
            cartFromDb.Count += cart.Count;
            this.unitOfWork.ShoppingCartRepository.Update(cartFromDb);
        }
        else
        {
            //Create Cart
            this.unitOfWork.ShoppingCartRepository.Add(cart);
            // Session
        }

        this.unitOfWork.Save();
        TempData["success"] = "Cart updated successfully";

        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
