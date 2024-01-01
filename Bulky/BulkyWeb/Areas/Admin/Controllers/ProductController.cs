namespace BulkyWeb.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Hosting;

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
            productVm.Product = this.unitOfWork.ProductRepository.Get(x => x.Id == id, "ProductImages");
        }

        return View(productVm);
    }

    [HttpPost]
    public IActionResult Upsert(ProductViewModel productVm, List<IFormFile> files)
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

            if (files != null)
            {
                var rootPath = this.webHostEnvironment.WebRootPath;

                foreach (var file in files)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = @"images\products\product-" + productVm.Product.Id;
                    string finalPath = Path.Combine(rootPath, productPath);

                    if (!Directory.Exists(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProductImage productImage = new()
                    {
                        ImageUrl = @"\" + productPath + @"\" + fileName,
                        ProductId = productVm.Product.Id,
                    };

                    this.unitOfWork.ProductImageRepository.Add(productImage);
                }

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

    public IActionResult DeleteImage(int imageId)
    {
        var imageToBeDeleted = this.unitOfWork.ProductImageRepository.Get(x => x.Id == imageId);
        int productId = imageToBeDeleted.ProductId;

        if (imageToBeDeleted != null)
        {
            if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(this.webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            this.unitOfWork.ProductImageRepository.Remove(imageToBeDeleted);
            this.unitOfWork.Save();

            TempData["success"] = "Image Deleted successfully";
        }

        return RedirectToAction(nameof(Upsert), new { id = productId });
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

        string productPath = @"images\products\product-" + id;
        string finalPath = Path.Combine(this.webHostEnvironment.WebRootPath, productPath);

        if (Directory.Exists(finalPath))
        {
            string[] filePaths = Directory.GetFiles(finalPath);

            foreach (string filePath in filePaths)
            {
                System.IO.File.Delete(filePath);
            }

            Directory.Delete(finalPath);
        }

        this.unitOfWork.ProductRepository.Remove(product);
        this.unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}
