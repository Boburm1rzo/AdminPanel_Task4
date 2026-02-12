using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Task4.Services;

namespace Task4.Pages.Account
{
    public class ResetPasswordModel(AuthService authService) : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Token { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [MinLength(8)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet(string? token, string? email)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
                return BadRequest("Invalid reset link.");

            Input.Email = email;
            Input.Token = token.Replace(" ", "+");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var ok = await authService.ResetPasswordAsync(Input.Email, Input.Token, Input.Password);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Reset link is invalid or expired.");
                return Page();
            }

            TempData["StatusMessage"] = "Password successfully changed. Please sign in.";
            return RedirectToPage("/Account/Login");
        }
    }

}
