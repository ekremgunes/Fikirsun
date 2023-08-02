using Fikirsun.DAL.Context;
using Fikirsun.Entities;
using Fikirsun.Tools.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Database connection
builder.Services.AddDbContext<FikirsunContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Local")
        , o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});


#region Identity & Cookie


builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<FikirsunContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._������������";
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 4;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(23); // oturum s�resi 23 saat
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        Name = ".Fikirsun.S.C."
    };
});

#endregion

#region UserDataSeed

using (var serviceProvider = builder.Services.BuildServiceProvider())
{
    var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

    SeedData seedData = new(userManager, roleManager);

    // veritaban� olu�turmadan bu metodu yorum sat�r� yap , sonra kald�r
    //seedData.Initialize().Wait();
}

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{

    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

});

app.Run();
