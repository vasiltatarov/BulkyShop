namespace Bulky.Data.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail entity);
}
