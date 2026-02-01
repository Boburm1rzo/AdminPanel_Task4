using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Task4.Services;

namespace Task4.Pages.Account
{
    public sealed class LoginModel(AuthService authService) : PageModel
    {

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [Required]
            public required string Password { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await authService.Login(Input.Email, Input.Password);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Login failed. Invalid email or password.");
                return Page();
            }

            return RedirectToPage("/Admin/Users");
        }
    }
}
