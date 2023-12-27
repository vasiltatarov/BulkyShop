namespace Bulky.Data.Repository;

public class ShopingCartRepository : Repository<ShopingCart>, IShopingCartRepository
{
    private readonly ApplicationDbContext dbContext;

    public ShopingCartRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(ShopingCart entity)
    {
        this.dbContext.ShopingCarts.Update(entity);
    }
}
