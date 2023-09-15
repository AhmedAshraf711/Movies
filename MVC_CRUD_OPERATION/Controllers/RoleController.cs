using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_CRUD_OPERATION.Models;
using MVC_CRUD_OPERATION.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_CRUD_OPERATION.Controllers
{
    public class RoleController : Controller
    {
       
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext applicationDbContext;

        public RoleController(RoleManager<IdentityRole> roleManager,ApplicationDbContext applicationDbContext)
        {
            this.roleManager = roleManager;
            this.applicationDbContext = applicationDbContext;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
           ViewBag.Roles=applicationDbContext.Roles.ToList();
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> New(RoleViewModel roleViewModel) 
        { 
            if (ModelState.IsValid)
            {
            IdentityRole roleModel = new IdentityRole();
            roleModel.Name = roleViewModel.RoleName;
                    //sv db
            IdentityResult result=await roleManager.CreateAsync(roleModel);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(roleViewModel);
        }
    }
}
