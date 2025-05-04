using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjeAspNETCORE.Models;
using ProjeAspNETCORE;

var builder = WebApplication.CreateBuilder(args);

// Добавляем DbContext — обязательно!
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ??
                         "Server=(localdb)\\MSSQLLocalDB;Database=CarCompany;Trusted_Connection=True;");
});

// Настройка Identity
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

var app = builder.Build();

// Конфигурация middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 🔥 ОБЯЗАТЕЛЬНО до UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");
SeedAdminUser(app);
app.Run();

void SeedAdminUser(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();

        // Убедимся, что роль Admin существует
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

		// Проверим, существует ли уже пользователь
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
                Console.WriteLine("✔ Админ создан успешно!");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"❌ Ошибка: {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("ℹ Админ уже существует.");
        }
    }
}