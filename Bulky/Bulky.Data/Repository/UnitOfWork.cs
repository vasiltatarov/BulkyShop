namespace Bulky.Data.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.ApplicationUserRepository = new ApplicationUserRepository(dbContext);
        this.CategoryRepository = new CategoryRepository(dbContext);
        this.ProductRepository = new ProductRepository(dbContext);
        this.CompanyRepository = new CompanyRepository(dbContext);
        this.ShopingCartRepository = new ShopingCartRepository(dbContext);
    }

    public ICategoryRepository CategoryRepository { get; private set; }

    public IProductRepository ProductRepository { get; private set; }

    public ICompanyRepository CompanyRepository { get; private set; }

    public IShopingCartRepository ShopingCartRepository { get; private set; }

    public IApplicationUserRepository ApplicationUserRepository { get; private set; }

    public void Save()
    {
        this.dbContext.SaveChanges();
    }
}
