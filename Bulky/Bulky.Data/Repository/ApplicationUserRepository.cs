namespace Bulky.Data.Repository;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext dbContext;

    public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
	{
        this.dbContext = dbContext;
	}

    public void Update(ApplicationUser entity)
    {
        this.dbContext.Update(entity);
    }
}
