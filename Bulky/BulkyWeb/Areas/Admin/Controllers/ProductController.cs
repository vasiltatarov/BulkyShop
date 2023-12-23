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

    public IActionResult Upsert(int? id)
    {
        var productVm = new ProductViewModel
        {
            CategoryList = this.unitOfWork.CategoryRepository
                .GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToList(),
            Product = new()
        };

        if (id != null && id != 0)
        {
            productVm.Product = this.unitOfWork.ProductRepository.Get(x => x.Id == id);
        }

        return View(productVm);
    }

    [HttpPost]
    public IActionResult Upsert(ProductViewModel productVm)
    {
        if (ModelState.IsValid)
        {
            if (productVm.Product.Id != 0)
            {
                this.unitOfWork.ProductRepository.Add(productVm.Product);
                TempData["success"] = string.Format(WebConstants.SuccessCreateNotification, nameof(Product));
            }
            else
            {
                this.unitOfWork.ProductRepository.Update(productVm.Product);
                TempData["success"] = string.Format(WebConstants.SuccessEditNotification, nameof(Product));
            }

            this.unitOfWork.Save();

            return RedirectToAction("Index");
        }
        else
        {
            productVm.CategoryList = this.unitOfWork.CategoryRepository
                .GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToList();

            return View(productVm);
        }
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
