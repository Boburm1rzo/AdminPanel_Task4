using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Task4.Models;
using Task4.Services;

namespace Task4.FIlters;

public class UserStatusMiddleware
{
    private readonly RequestDelegate _next;

    public UserStatusMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserService userService)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out var userId))
            {
                await ForceLogout(context);
                return;
            }

            User dbUser;
            try
            {
                dbUser = await userService.GetUserByIdAsync(userId);
            }
            catch
            {
                await ForceLogout(context);
                return;
            }

            if (dbUser.Status == UserStatus.Blocked)
            {
                await ForceLogout(context);
                return;
            }
        }

        await _next(context);
    }

    private static async Task ForceLogout(HttpContext context)
    {
        await context.SignOutAsync();
        context.Response.Redirect("/account/login");
    }
}
