using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Task4.Services;

namespace Task4.Pages.Account
{
    public class ForgotPasswordModel(
        AuthService authService,
        EmailService emailService) : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var token = await authService.CreateResetTokenAsync(Input.Email);

            if (token is not null)
            {
                var resetLink = Url.Page(
                    pageName: "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { token, email = Input.Email },
                    protocol: Request.Scheme);

                await emailService.SendResetPasswordLinkAsync(Input.Email, resetLink!);
            }
            TempData["StatusMessage"] =
                "If an account with this email exists, a reset link has been sent.";

            return RedirectToPage();
        }
    }
}
