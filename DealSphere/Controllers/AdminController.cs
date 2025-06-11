using DealSphere.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DealSphere.ViewModels;
using DealSphere.Models;


namespace DealSphere.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                TotalDeals = await _context.Deals.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalAdmins = (await _userManager.GetUsersInRoleAsync("Admin")).Count,
                PublishedDeals = _context.Deals.Count(d => d.IsPublished == true),
                UnPublishedDeals = _context.Deals.Count(d => !d.IsPublished),
                RecentDeals = await _context.Deals
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ManageDeals(string search, string status)
        {
            var deals = _context.Deals.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                deals = deals.Where(d => d.Title.Contains(search));
                ViewBag.Search = search;
            }

            if (!string.IsNullOrEmpty(status))
            {
                bool isPublished = status == "Published";
                deals = deals.Where(d => d.IsPublished == isPublished);
                ViewBag.Status = status;
            }

            var result = await deals.OrderByDescending(d => d.CreatedAt).ToListAsync();
            return View("ManageDeals", result); // This must match your .cshtml filename
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<ManageUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new ManageUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Roles = roles.ToList(),
                     CreatedAt = user.CreatedAt
                });
            }

            return View(userViewModels);
        }


        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.Email;
            user.Name = model.Name; // Only if you have this field

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("ManageUsers");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(user);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction("ManageUsers");
        }
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userManager.AddToRoleAsync(user, "Admin");
            return RedirectToAction("ManageUsers");
            TempData["Success"] = $"{user.Email} is now an admin.";

        }

        [HttpPost]
        public async Task<IActionResult> RemoveAdminRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userManager.RemoveFromRoleAsync(user, "Admin");
            return RedirectToAction("ManageUsers");
        }

    }
}
