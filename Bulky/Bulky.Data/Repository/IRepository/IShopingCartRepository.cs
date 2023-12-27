namespace Bulky.Data.Repository.IRepository;

public interface IShopingCartRepository : IRepository<ShopingCart>
{
    void Update(ShopingCart entity);
}
