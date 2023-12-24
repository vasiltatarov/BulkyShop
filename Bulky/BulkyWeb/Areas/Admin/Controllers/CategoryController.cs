namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var categories = unitOfWork.CategoryRepository.GetAll().OrderBy(x => x.DisplayOrder).ToList();

        return View(categories);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            this.unitOfWork.CategoryRepository.Add(category);
            this.unitOfWork.Save();

            TempData["success"] = string.Format(WebConstants.SuccessCreateNotification, nameof(Category));

            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Edit(int id)
    {
        var category = this.unitOfWork.CategoryRepository.Get(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            this.unitOfWork.CategoryRepository.Update(category);
            this.unitOfWork.Save();

            TempData["success"] = string.Format(WebConstants.SuccessEditNotification, nameof(Category));

            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Delete(int id)
    {
        var category = this.unitOfWork.CategoryRepository.Get(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int id)
    {
        var category = this.unitOfWork.CategoryRepository.Get(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        this.unitOfWork.CategoryRepository.Remove(category);
        this.unitOfWork.Save();

        TempData["success"] = string.Format(WebConstants.SuccessDeleteNotification, nameof(Category));

        return RedirectToAction("Index");
    }
}
