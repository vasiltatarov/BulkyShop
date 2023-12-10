namespace Bulky.Data.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext dbContext;

    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Save()
    {
        this.dbContext.SaveChanges();
    }

    public void Update(Category category)
    {
        this.dbContext.Categories.Update(category);
    }
}
