namespace Bulky.Data.Repository;

public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
    private readonly ApplicationDbContext dbContext;

    public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(ShoppingCart entity)
    {
        this.dbContext.ShoppingCarts.Update(entity);
    }
}
