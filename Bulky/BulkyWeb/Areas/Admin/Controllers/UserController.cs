namespace BulkyWeb.Areas.Admin.Controllers;

[Area(SD.Role_Admin)]
[Authorize]
public class UserController : Controller
{
    private readonly IUnitOfWork unitOfWork;
    private readonly UserManager<IdentityUser> userManager;

    public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
    {
        this.unitOfWork = unitOfWork;
        this.userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    #region Api Calls

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = this.unitOfWork.ApplicationUserRepository.GetAll();
        var companies = this.unitOfWork.CompanyRepository.GetAll();

        foreach (var user in users)
        {
            if (user.CompanyId.HasValue && user.CompanyId.Value != 0)
            {
                user.UserCompanyName = companies.FirstOrDefault(x => x.Id == user.CompanyId)?.Name;
            }
            else
            {
                user.UserCompanyName = string.Empty;
            }

            user.Role = this.userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
        }

        return Json(new { data = users });
    }

    [HttpPost]
    public IActionResult LockUnlock([FromBody] string id)
    {
        var user = this.unitOfWork.ApplicationUserRepository.Get(x => x.Id == id);
        var userLocked = "Unlocked";

        if (user == null)
        {
            return Json(new { success = false, message = "Error while Locking/Unlocking" });
        }

        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
        {
            //user is currently locked and we need to unlock them
            user.LockoutEnd = DateTime.Now;
        }
        else
        {
            userLocked = "Locked";
            user.LockoutEnd = DateTime.Now.AddYears(100);
        }

        this.unitOfWork.ApplicationUserRepository.Update(user);
        this.unitOfWork.Save();

        return Json(new { success = true, message = $"User is {userLocked} Successful" });
    }

    #endregion
}
