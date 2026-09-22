using InventorySystem.Models;
using InventorySystem.Services;
using Microsoft.EntityFrameworkCore;

// Ensure the physical web root exists before ASP.NET Core starts loading
// static web assets. This makes the project portable after ZIP/GitHub copy
// and prevents DirectoryNotFoundException when wwwroot is missing locally.
var contentRoot = Directory.GetCurrentDirectory();
var webRoot = Path.Combine(contentRoot, "wwwroot");
Directory.CreateDirectory(webRoot);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot,
    WebRootPath = webRoot
});

// MVC
builder.Services.AddControllersWithViews();

// Entity Framework Core / SQL Server
builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Connection string 'DefaultConnection' was not found in appsettings.json.")));

// AI Inventory Assistant
builder.Services.AddScoped<IAIAssistantService, AIAssistantService>();

var app = builder.Build();

// HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
