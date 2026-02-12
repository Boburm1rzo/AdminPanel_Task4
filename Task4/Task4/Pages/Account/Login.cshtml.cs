using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Task4.Models;
using Task4.Services;

namespace Task4.Pages.Account
{
    public sealed class LoginModel(
        AuthService authService,
        UserService userService) : PageModel
    {

        [BindProperty]
        public required InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [Required]
            public required string Password { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int confirmUserId)
        {
            try
            {
                if (confirmUserId == 0)
                    return Page();

                var user = await userService.GetUserByIdAsync(confirmUserId);

                if (user is null)
                    return Page();

                if (user.Status == UserStatus.Unverified)
                {
                    await userService.UpdateUserStatusAsync(confirmUserId, UserStatus.Active);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");
                return Page();
            }

            TempData["StatusMessage"] =
                "Your e-mail has been successfully confirmed.";

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            User user;

            try
            {
                user = await authService.LoginAsync(Input.Email, Input.Password);
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            await userService.UpdateLastSeenAsync(user.Id);

            return RedirectToPage("/Index");
        }
    }
}
