namespace BulkyWeb.Areas.Admin.Controllers;

[Area(SD.Role_Admin)]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private readonly IUnitOfWork unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public IActionResult Index() => View();

    public IActionResult Upsert(int? id)
    {
        if (id == null || id == 0)
        {
            return View(new Company());
        }
        else
        {
            var company = this.unitOfWork.CompanyRepository.Get(x => x.Id == id);

            return View(company);
        }
    }

    [HttpPost]
    public IActionResult Upsert(Company company)
    {
        if (ModelState.IsValid)
        {
            if (company.Id == 0)
            {
                this.unitOfWork.CompanyRepository.Add(company);
            }
            else
            {
                this.unitOfWork.CompanyRepository.Update(company);
            }

            this.unitOfWork.Save();
            TempData["success"] = string.Format(WebConstants.SuccessCreateNotification, nameof(Company));

            return RedirectToAction("Index");
        }
        else
        {
            return View(company);
        }
    }

    #region Api Calls
    public IActionResult GetAll()
    {
        var companies = this.unitOfWork.CompanyRepository.GetAll();

        return Json(new { data = companies });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var company = this.unitOfWork.CompanyRepository.Get(x => x.Id == id);

        if (company == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        this.unitOfWork.CompanyRepository.Remove(company);
        this.unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }
    #endregion
}
