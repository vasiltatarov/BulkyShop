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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            TempData["success"] = "Category created successfully";
            unitOfWork.CategoryRepository.Add(category);
            unitOfWork.Save();

            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Edit(int id)
    {
        var category = unitOfWork.CategoryRepository.Get(x => x.Id == id);

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
            TempData["success"] = "Category updated successfully";
            unitOfWork.CategoryRepository.Update(category);
            unitOfWork.Save();

            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Delete(int id)
    {
        var category = unitOfWork.CategoryRepository.Get(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int id)
    {
        var category = unitOfWork.CategoryRepository.Get(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        unitOfWork.CategoryRepository.Remove(category);
        unitOfWork.Save();
        TempData["success"] = "Category deleted successfully";

        return RedirectToAction("Index");
    }
}
