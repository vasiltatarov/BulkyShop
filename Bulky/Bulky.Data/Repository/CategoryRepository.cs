namespace Bulky.Data.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext dbContext;

    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(Category entity)
    {
        this.dbContext.Categories.Update(entity);
    }
}
