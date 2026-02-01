using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Task4.Data;
using Task4.Models;

namespace Task4.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UsersModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public IList<User> Users { get; private set; } = new List<User>();

        public async Task OnGetAsync()
        {
            Users = await _context.Users
                .OrderByDescending(u => u.LastSeen)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
