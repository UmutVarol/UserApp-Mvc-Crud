using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using UserApp.Data;
using UserApp.Data.Seed;
using UserApp.Entities;
using UserApp.Services;
using UserApp.Services.Validation;
using UserApp.Entities.Dtos;
using UserApp.Web.Authorization;
using UserApp.Web.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageKullanici", policy =>
        policy.Requirements.Add(new CanManageKullaniciRequirement()));

    options.AddPolicy("CanViewKullaniciDetail", policy =>
        policy.Requirements.Add(new CanViewKullaniciDetailRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, CanManageKullaniciHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanViewKullaniciDetailHandler>();

builder.Services.AddScoped<IKullaniciRepository, KullaniciRepository>();
builder.Services.AddScoped<IDepartmanRepository, DepartmanRepository>();
builder.Services.AddScoped<IValidator<KullaniciCreateDto>, KullaniciCreateDtoValidator>();
builder.Services.AddScoped<IValidator<KullaniciEditDto>, KullaniciEditDtoValidator>();
builder.Services.AddScoped<IValidator<KullaniciSelfEditDto>, KullaniciSelfEditDtoValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<IFileHelper, FileHelper>();
var app = builder.Build();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

app.Run();