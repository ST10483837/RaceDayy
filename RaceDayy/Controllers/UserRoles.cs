using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDayy.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using RaceDayy.Data;


namespace RaceDayy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRoles : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoles(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //Get: /UserRoles
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var thisViewModel = new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = await _userManager.GetRolesAsync(user)

                };
                userRolesViewModel.Add(thisViewModel);

            }
            return View(userRolesViewModel);
        }

        //Get: /UserRoles/Manage/userId=xxx
        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("Not Found");
            }

            ViewBag.UserName = user.UserName;
            var model = new List<ManageUserRoleViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name ?? "",

                };

                if (await _userManager.IsInRoleAsync(user, role.Name ?? ""))
                {
                    userRolesViewModel.isSelected = true;
                }
                else
                {
                    userRolesViewModel.isSelected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
    
    //Post: /UserRoles/Manage
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(List<ManageUserRoleViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Not Found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles!");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user,model.Where(x => x.isSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");
        }
    }
}