namespace BulkyWeb.Areas.Admin.Controllers;

using Bulky.Models;
using Microsoft.AspNetCore.Identity;

[Area(SD.Role_Admin)]
[Authorize]
public class UserController : Controller
{
    private readonly IUnitOfWork unitOfWork;
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.unitOfWork = unitOfWork;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult RoleManagment(string userId)
    {
        var user = this.unitOfWork.ApplicationUserRepository.Get(x => x.Id == userId);
        var viewModel = new RoleManagmentViewModel
        {
            User = user,
            Companies = this.unitOfWork.CompanyRepository.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }),
            Roles = this.roleManager.Roles.Select(x => new SelectListItem
            {
                Text= x.Name,
                Value = x.Name
            }),
        };

        viewModel.User.Role = this.userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult RoleManagment(RoleManagmentViewModel roleManagmentViewModel)
    {
        var user = this.unitOfWork.ApplicationUserRepository.Get(x => x.Id == roleManagmentViewModel.User.Id);
        var oldRole = this.userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

        if (roleManagmentViewModel.User.Role == oldRole)
        {
            if (oldRole == SD.Role_Company && user.CompanyId != roleManagmentViewModel.User.CompanyId)
            {
                user.CompanyId = roleManagmentViewModel.User.CompanyId;
                this.unitOfWork.ApplicationUserRepository.Update(user);
                this.unitOfWork.Save();
            }
        }
        else
        {
            if (roleManagmentViewModel.User.Role == SD.Role_Company)
            {
                user.CompanyId = roleManagmentViewModel.User.CompanyId;
            }

            if (oldRole == SD.Role_Company)
            {
                user.CompanyId = null;
            }

            this.unitOfWork.ApplicationUserRepository.Update(user);
            this.unitOfWork.Save();

            this.userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
            this.userManager.AddToRoleAsync(user, roleManagmentViewModel.User.Role).GetAwaiter().GetResult();
        }

        return RedirectToAction("Index");
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
