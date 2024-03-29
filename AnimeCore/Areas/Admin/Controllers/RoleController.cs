using System.Linq;
using System.Threading.Tasks;
using AnimeCore.Common;
using Entities.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.RoleViewModels;
using Services;

namespace AnimeCore.Areas.Admin.Controllers
{
    public class RoleController : AdminIdentityController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public IActionResult Index()
        {
            var model = _roleService.ToList().Select(x => new RoleListViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                NumberOfUsers = x.Users.Count
            });
            return View(model);
        }

        public async Task<IActionResult> AddEdit(string id)
        {
            var model = new RoleViewModel();
            var role = await _roleService.FindByIdAsync(id);
            if (role != null)
            {
                model.Id = role.Id;
                model.Name = role.Name;
                model.Description = role.Description;
            }
            return PartialView("_AddEditPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleService.FindByIdAsync(model.Id);
                IdentityResult result;
                if (role != null)
                {
                    role.Name = model.Name;
                    role.Description = model.Description;
                    result = await _roleService.UpdateAsync(role);
                }
                else
                {
                    role = new Role
                    {
                        Name = model.Name,
                        Description = model.Description
                    };
                    result = await _roleService.CreateAsync(role);
                }
                if (result.Succeeded)
                {
                    return JsonStatus.Ok;
                }
                AddErrors(result);
            }
            return PartialView("_AddEditPartial", model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleService.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var model = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
            return PartialView("_DeletePartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RoleViewModel model)
        {
            var role = await _roleService.FindByIdAsync(model.Id);
            if (role == null)
            {
                return NotFound();
            }
            var result = await _roleService.DeleteAsync(role);
            if (result.Succeeded)
            {
                return JsonStatus.Ok;
            }
            AddErrors(result);
            return PartialView("_DeletePartial", model);
        }
    }
}