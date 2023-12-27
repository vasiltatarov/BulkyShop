namespace BulkyWeb.Areas.Admin.Controllers;

[Area(SD.Role_Admin)]
public class ProductController : Controller
{
    private const string ImagesPath = @"images\products";

    private readonly IUnitOfWork unitOfWork;
    private readonly IWebHostEnvironment webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        this.unitOfWork = unitOfWork;
        this.webHostEnvironment = webHostEnvironment;
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
    public IActionResult Upsert(ProductViewModel productVm, IFormFile file)
    {
        if (ModelState.IsValid)
        {
            if (productVm.Product.Id == 0)
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

            if (file != null)
            {
                var rootPath = this.webHostEnvironment.WebRootPath;
                var fileName = productVm.Product.Id.ToString() + "-" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var productPath = Path.Combine(rootPath, ImagesPath);
                var finalPath = Path.Combine(productPath, fileName);

                if (!string.IsNullOrEmpty(productVm.Product.ImageUrl))
                {
                    var oldImagePath = Path.Combine(productPath, productVm.Product.ImageUrl);

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                if (!Directory.Exists(productPath))
                {
                    Directory.CreateDirectory(productPath);
                }

                using (var fileStream = new FileStream(finalPath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productVm.Product.ImageUrl = fileName;
                this.unitOfWork.ProductRepository.Update(productVm.Product);
                this.unitOfWork.Save();
            }

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

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var product = this.unitOfWork.ProductRepository.Get(x => x.Id == id);

        if (product == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var rootPath = this.webHostEnvironment.WebRootPath;
            var productPath = Path.Combine(rootPath, ImagesPath);
            var finalPath = Path.Combine(productPath, product.ImageUrl);

            if (System.IO.File.Exists(finalPath))
            {
                System.IO.File.Delete(finalPath);
            }
        }

        this.unitOfWork.ProductRepository.Remove(product);
        this.unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}
