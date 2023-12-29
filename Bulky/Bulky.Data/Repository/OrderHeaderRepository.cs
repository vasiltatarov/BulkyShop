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

    public void UpdateStatus(int id, string orderStatus, string paymentStatus = null)
    {
        var orderHeader = this.dbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);

        if (orderHeader != null)
        {
            orderHeader.OrderStatus = orderStatus;

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                orderHeader.PaymentStatus = paymentStatus;
            }
        }
    }

    public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
    {
        var orderHeader = this.dbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);

        if (!string.IsNullOrEmpty(sessionId))
        {
            orderHeader.SessionId = sessionId;
        }

        if (!string.IsNullOrEmpty(paymentIntentId))
        {
            orderHeader.PaymentIntentId = paymentIntentId;
            orderHeader.PaymentDate = DateTime.Now;
        }
    }
}
