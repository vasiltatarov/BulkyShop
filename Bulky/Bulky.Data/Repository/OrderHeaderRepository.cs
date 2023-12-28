namespace Bulky.Data.Repository;

using Bulky.Data.Data;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext dbContext;

    public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(OrderHeader entity)
    {
        this.dbContext.OrderHeaders.Update(entity);
    }
}
