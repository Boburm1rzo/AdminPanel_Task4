using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Task4.Models;
using Task4.Services;

namespace Task4.Pages.Admin
{
    public class UsersModel(UserService userService) : PageModel
    {
        public List<User> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            Users = await userService.GetAllUsers();
        }

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
    }
}
