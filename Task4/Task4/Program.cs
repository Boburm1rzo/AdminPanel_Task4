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
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/account/login";
    });

builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserStatusMiddleware>();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/", context =>
{
    context.Response.Redirect("/admin/users");
    return Task.CompletedTask;
});

app.Run();
