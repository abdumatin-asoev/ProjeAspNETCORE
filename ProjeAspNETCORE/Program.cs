using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjeAspNETCORE.Models;
using ProjeAspNETCORE;

var builder = WebApplication.CreateBuilder(args);

// Adding DbContext — mandatory!
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=ARSALANKHROUSH7;Database=CarCompany;Trusted_Connection=True;TrustServerCertificate=True;");
});

// Enabling Validation for the entire application
builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
            _ => "This field is required.");
    });

// Configuring Identity
builder.Services
    .AddIdentity<User, IdentityRole<long>>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication(); // 🔥 MANDATORY before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}");
SeedAdminUser(app);
app.Run();

void SeedAdminUser(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

        // Ensure the Admin role exists
        var adminRoleExists = roleManager.RoleExistsAsync("Admin").Result;
        if (!adminRoleExists)
        {
            roleManager.CreateAsync(new IdentityRole<long>("Admin")).Wait();
        }
        var customerRoleExists = roleManager.RoleExistsAsync("Customer").Result;
        if (!customerRoleExists)
        {
            roleManager.CreateAsync(new IdentityRole<long>("Customer")).Wait();
        }

        // Check if the admin user already exists
        var adminUser = userManager.FindByEmailAsync("admin@mail.com").Result;
        if (adminUser == null)
        {
            var user = new User
            {
                UserName = "admin@mail.com",
                Email = "admin@mail.com",
                Role = UserRole.Admin,
                FirstName = "Admin",
                LastName = "Boss"
            };

            var result = userManager.CreateAsync(user, "123456").Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, "Admin").Wait();
                Console.WriteLine("✔ Admin created successfully!");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"❌ Error: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("ℹ Admin already exists.");
        }
    }
}