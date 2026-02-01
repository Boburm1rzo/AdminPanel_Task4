using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Task4.Services;

namespace Task4.FIlters;

public class CheckUserStatusFilter(UserService service) : IAsyncPageFilter
{
    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        var path = context.HttpContext.Request.Path.Value?.ToLower();
        if (path.Contains("/account/login") || path.Contains("/account/register"))
        {
            await next();
            return;
        }

        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                var isActive = await service.IsUserActiveAsync(userId);
                if (!isActive)
                {
                    await context.HttpContext.SignOutAsync();
                    context.Result = new RedirectToPageResult("/Account/Login");
                    return;
                }
            }
        }

        await next();
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;
}
