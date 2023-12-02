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
        var categories = db.Categories.ToList();

        return this.View(categories);
    }
}
