using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task4.Data;
using Task4.FIlters;
using Task4.Models;
using Task4.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options => options.LoginPath = "/account/login");

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();

var cs = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(cs))
    throw new Exception("DefaultConnection is missing.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(cs));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        Console.WriteLine("Database migration muvaffaqiyatli");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration xatosi: {ex.Message}");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
    });

    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/health"))
    {
        ctx.Response.StatusCode = 200;
        await ctx.Response.WriteAsync("OK");
        return;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserStatusMiddleware>();

app.MapRazorPages();

app.Run();
