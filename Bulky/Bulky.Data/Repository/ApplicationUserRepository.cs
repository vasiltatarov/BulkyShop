namespace Bulky.Data.Repository;

public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
	public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
	{
	}
}
