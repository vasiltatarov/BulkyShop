namespace Bulky.Data.Repository.IRepository;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    void Update(ShoppingCart entity);
}
