using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using LastTask1.Models;
using LastTask1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace CustomIdentityApp.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);
            List<User> Users = _userManager.Users.ToList();
            AdminPanelViewModel model = new AdminPanelViewModel()
            {
                User = user,
                Users = Users
            };
            return View(model);
        }
 
        public async Task<IActionResult> ChangeRole(string role, string userName)
        {
            User user = await _userManager.FindByNameAsync(userName);
            user.Role = role;

            var result = await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", "Users", new { userName = userName});
        }
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index", new { userName = user.UserName});
        }
    }
}