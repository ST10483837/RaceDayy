using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDayy.Data;
using RaceDayy.Models; // <-- Added for ApplicationUser
using RaceDayy.Models.Interface;
using RaceDayy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IProfileImageService, ProfileImageService>();

// Register the Azure Blob Storage Service
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

// Add MVC and Swagger
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer(); // <-- REQUIRED FOR SWAGGER
builder.Services.AddSwaggerGen();           // <-- REQUIRED FOR SWAGGER

var app = builder.Build();

// Seed Roles and Admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Organizer", "Participant" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    string email = "admin@admin.com";
    string password = "Admin1@";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "Admin");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // <-- ENABLES SWAGGER JSON
    app.UseSwaggerUI();    // <-- ENABLES THE SWAGGER WEB PAGE
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map MVC default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages (Identity)
app.MapRazorPages();

// Map API Controllers (for Swagger testing)
app.MapControllers();

app.Run();