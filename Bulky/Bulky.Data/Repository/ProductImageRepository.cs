namespace Bulky.Data.Repository;

public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    private readonly ApplicationDbContext dbContext;

    public ProductImageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(ProductImage entity)
    {
        this.dbContext.ProductImages.Update(entity);
    }
}
