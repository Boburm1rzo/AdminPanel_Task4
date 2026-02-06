using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Task4.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
       => RedirectToPage("/admin/users");
    }
}
