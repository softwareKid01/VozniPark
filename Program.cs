using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VozniPark.Data;
using VozniPark.Models;
using VozniPark.Repositories;
using VozniPark.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Registracija baze podataka
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IVoziloService, VoziloService>();
builder.Services.AddScoped<IServisService, ServisService>();

// 2. Registracija Identity servisa
builder.Services.AddIdentity<Korisnik, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddErrorDescriber<CustomIdentityErrorDescriber>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repo<>));

// Konfiguracija putanja za prijavu
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// --- POCETAK SEEDINGA ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Automatsko kreiranje tabele u bazi ako ne postoji
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        // Seeding rola i admina
        await DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Greška prilikom inicijalizacije baze: " + ex.Message);
    }
}
// --- KRAJ SEEDINGA ---

app.Run();