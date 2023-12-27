namespace Bulky.Data.Repository.IRepository;

public interface ICompanyRepository : IRepository<Company>
{
    void Update(Company entity);
}
