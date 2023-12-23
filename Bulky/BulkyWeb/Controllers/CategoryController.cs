namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var categories = this.unitOfWork.CategoryRepository.GetAll().OrderBy(x => x.DisplayOrder).ToList();

        return this.View(categories);
    }

    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            TempData["success"] = "Category created successfully";
            this.unitOfWork.CategoryRepository.Add(category);
            this.unitOfWork.Save();

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
            TempData["success"] = "Category updated successfully";
            this.unitOfWork.CategoryRepository.Update(category);
            this.unitOfWork.Save();

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
        TempData["success"] = "Category deleted successfully";

        return RedirectToAction("Index");
    }
}
