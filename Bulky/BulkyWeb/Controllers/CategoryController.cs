namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryRepository categoryRepository;

    public CategoryController(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public IActionResult Index()
    {
        var categories = this.categoryRepository.GetAll().OrderBy(x => x.DisplayOrder).ToList();

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
            //_unitOfWork.Category.Add(category);
            //_unitOfWork.Save();

            TempData["success"] = "Category created successfully";
            this.categoryRepository.Add(category);
            this.categoryRepository.Save();

            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Edit(int id)
    {
        //var category = _unitOfWork.Category.Get(u => u.Id == id);
        var category = this.categoryRepository.Get(x => x.Id == id);
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
            //_unitOfWork.Category.Update(category);
            //_unitOfWork.Save();

            TempData["success"] = "Category updated successfully";
            this.categoryRepository.Update(category);
            this.categoryRepository.Save();

            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Delete(int id)
    {
        //Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
        var category = this.categoryRepository.Get(x => x.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int id)
    {
        //Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
        var category = this.categoryRepository.Get(x => x.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        //_unitOfWork.Category.Remove(obj);
        //_unitOfWork.Save();
        this.categoryRepository.Remove(category);
        this.categoryRepository.Save();
        TempData["success"] = "Category deleted successfully";

        return RedirectToAction("Index");
    }
}
