namespace Bulky.Data.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly ApplicationDbContext context;

    public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.context = context;
    }

    public void Initialize()
    {
        try
        {
            if (this.context.Database.GetPendingMigrations().Count() > 0)
            {
                this.context.Database.Migrate();
            }
        }
        catch (Exception)
        {
        }

        if (!(this.roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult()))
        {
            this.roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
            this.roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
            this.roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            this.roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

            this.userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name = "Admin Adminov",
                PhoneNumber = "1112223333",
                StreetAddress = "test 123 Ave",
                State = "IL",
                PostalCode = "23422",
                City = "Chakalo"
            }, "admin123#").GetAwaiter().GetResult();

            var user = this.context.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
            this.userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
        }

        return;
    }
}
