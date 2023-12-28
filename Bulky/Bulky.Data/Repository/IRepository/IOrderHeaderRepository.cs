namespace Bulky.Data.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader entity);
}
