namespace Bulky.Models.ViewModels;

using Microsoft.AspNetCore.Mvc.Rendering;

public class RoleManagmentViewModel
{
    public ApplicationUser User { get; set; }

    public IEnumerable<SelectListItem> Companies { get; set; }

    public IEnumerable<SelectListItem> Roles { get; set; }
}
