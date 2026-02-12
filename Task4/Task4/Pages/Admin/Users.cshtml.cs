using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Task4.Models;
using Task4.Services;

namespace Task4.Pages.Admin
{
    [Authorize]
    public class UsersModel(UserService userService) : PageModel
    {
        public List<User> Users { get; set; } = new();
        public string Sort { get; private set; } = "lastseen";
        public string Dir { get; private set; } = "desc";

        public async Task<IActionResult> OnPostUpdateStatusAsync(
            List<int> selectedUserIds,
            UserStatus status)
        {
            if (selectedUserIds == null || !selectedUserIds.Any())
            {
                TempData["ErrorMessage"] = "Please select at least one user";
                return RedirectToPage();
            }

            try
            {
                await userService.UpdateUsersStatusAsync(selectedUserIds, status);
                TempData["SuccessMessage"] = $"Successfully updated {selectedUserIds.Count} user(s) to {status}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating users: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await userService.DeleteUserAsync(id);
                TempData["SuccessMessage"] = "User deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMultipleAsync(List<int> selectedUserIds)
        {
            if (selectedUserIds == null || !selectedUserIds.Any())
            {
                TempData["ErrorMessage"] = "Please select at least one user";
                return RedirectToPage();
            }

            try
            {
                await userService.DeleteSelectedUsersAsync(selectedUserIds);
                TempData["SuccessMessage"] = $"Successfully deleted {selectedUserIds.Count} user(s)";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting users: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostClearUnverifiedAsync(int id)
        {
            try
            {
                await userService.DeleteUnverifiedUsersAsync(id);

                TempData["SuccessMessage"] = "All unverified users were deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting unverified users: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task OnGetAsync(string? sort, string? dir)
        {
            Sort = string.IsNullOrWhiteSpace(sort) ? "lastseen" : sort.ToLowerInvariant();
            Dir = (dir?.ToLowerInvariant() == "asc") ? "asc" : "desc";

            var users = await userService.GetAllUsers();

            Users = Sort switch
            {
                "name" => Dir == "asc"
                    ? users.OrderBy(x => x.Name).ToList()
                    : users.OrderByDescending(x => x.Name).ToList(),

                "email" => Dir == "asc"
                    ? users.OrderBy(x => x.Email).ToList()
                    : users.OrderByDescending(x => x.Email).ToList(),

                "lastseen" => Dir == "asc"
                    ? users.OrderBy(x => x.LastSeen).ToList()
                    : users.OrderByDescending(x => x.LastSeen).ToList(),

                _ => users.OrderByDescending(x => x.LastSeen).ToList()
            };
        }

        public string NextDir(string key)
           => Sort == key && Dir == "asc" ? "desc" : "asc";

        public string SortIcon(string key)
        {
            if (Sort != key) return "bi-arrow-down-up";
            return Dir == "asc" ? "bi-arrow-up" : "bi-arrow-down";
        }

    }
}
