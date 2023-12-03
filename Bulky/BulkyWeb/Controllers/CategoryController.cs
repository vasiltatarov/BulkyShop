namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext db;

    public CategoryController(ApplicationDbContext db)
    {
        this.db = db;
    }

    public IActionResult Index()
    {
        var categories = db.Categories.OrderBy(x => x.DisplayOrder).ToList();

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
            this.db.Categories.Add(category);
            this.db.SaveChanges();

            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Edit(int id)
    {
        //var category = _unitOfWork.Category.Get(u => u.Id == id);
        var category = this.db.Categories.FirstOrDefault(x => x.Id == id);
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
            this.db.Categories.Update(category);
            this.db.SaveChanges();

            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Delete(int id)
    {
        //Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
        var category = this.db.Categories.FirstOrDefault(x => x.Id == id);
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
        var category = this.db.Categories.FirstOrDefault(x => x.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        //_unitOfWork.Category.Remove(obj);
        //_unitOfWork.Save();
        this.db.Categories.Remove(category);
        this.db.SaveChanges();
        TempData["success"] = "Category deleted successfully";

        return RedirectToAction("Index");
    }
}
