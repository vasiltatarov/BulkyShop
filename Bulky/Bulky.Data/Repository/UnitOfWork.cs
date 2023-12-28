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
        this.ShoppingCartRepository = new ShoppingCartRepository(dbContext);
        this.OrderHeaderRepository = new OrderHeaderRepository(dbContext);
        this.OrderDetailRepository = new OrderDetailRepository(dbContext);
    }

    public ICategoryRepository CategoryRepository { get; private set; }

    public IProductRepository ProductRepository { get; private set; }

    public ICompanyRepository CompanyRepository { get; private set; }

    public IShoppingCartRepository ShoppingCartRepository { get; private set; }

    public IApplicationUserRepository ApplicationUserRepository { get; private set; }

    public IOrderHeaderRepository OrderHeaderRepository { get; private set; }

    public IOrderDetailRepository OrderDetailRepository { get; private set; }

    public void Save()
    {
        this.dbContext.SaveChanges();
    }
}
