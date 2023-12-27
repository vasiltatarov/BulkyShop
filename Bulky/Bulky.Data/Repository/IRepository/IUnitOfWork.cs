namespace Bulky.Data.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }

    IProductRepository ProductRepository { get; }

    ICompanyRepository CompanyRepository { get; }

    IShopingCartRepository ShopingCartRepository { get; }

    IApplicationUserRepository ApplicationUserRepository { get; }

    void Save();
}
