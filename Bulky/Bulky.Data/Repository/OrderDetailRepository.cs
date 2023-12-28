namespace Bulky.Data.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext dbContext;

    public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(OrderDetail entity)
    {
        this.dbContext.OrderDetails.Update(entity);
    }
}
