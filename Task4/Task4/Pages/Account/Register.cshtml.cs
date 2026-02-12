using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Task4.Models;
using Task4.Services;

namespace Task4.Pages.Account
{
    public class RegisterModel(
        AuthService authService,
        EmailService emailService) : PageModel
    {
        [BindProperty]
        public required InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public required string Name { get; set; }

            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [Required]
            public required string Password { get; set; }

            [Required]
            public required string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Input.ConfirmPassword != Input.Password)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            var user = new User
            {
                Email = Input.Email,
                Name = Input.Name,
                Status = UserStatus.Unverified,
                LastSeen = DateTime.UtcNow
            };

            try
            {
                await authService.RegisterAsync(user, Input.Password);

                var confirmationLink = Url.Page(
                    "/Account/Login",
                    pageHandler: null,
                    values: new { confirmUserId = user.Id },
                    protocol: Request.Scheme);

                _ = Task.Run(() => emailService.SendConfirmationEmailAsync(user.Email, confirmationLink!));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
                                      (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "A user with this e-mail already exists.");
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "An unexpected error occurred. Please try again later.");
                return Page();
            }

            TempData["StatusMessage"] =
                     "Registration successful. Please check your email to confirm your account.";

            return RedirectToPage("/Account/Login");
        }

        public void OnGet()
        {
        }
    }
}
