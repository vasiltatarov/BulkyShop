namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public IActionResult Index() => View();

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            this.unitOfWork.ProductRepository.Add(product);
            this.unitOfWork.Save();

            TempData["success"] = string.Format(WebConstants.SuccessCreateNotification, nameof(Product));

            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Edit(int id)
    {
        var product = this.unitOfWork.ProductRepository.Get(x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            this.unitOfWork.ProductRepository.Update(product);
            this.unitOfWork.Save();

            TempData["success"] = string.Format(WebConstants.SuccessEditNotification, nameof(Product));

            return RedirectToAction("Index");
        }

        return View();
    }

    #region Api Calls

    [HttpGet]
    public IActionResult GetAll()
    {
        var products = this.unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();

        return Json(new { data = products });
    }

    #endregion
}
