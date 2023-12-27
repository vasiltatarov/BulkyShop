namespace Bulky.Data.Repository;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    private readonly ApplicationDbContext dbContext;

    public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Update(Company entity)
    {
        this.dbContext.Companies.Update(entity);
    }
}
