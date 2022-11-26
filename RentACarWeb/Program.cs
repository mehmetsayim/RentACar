using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using RentACarData;
using RentACarWeb;
using ServiceStack.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContext>(config =>
{

    config.UseLazyLoadingProxies();

    var provider = builder.Configuration.GetValue<string>("DbProvider");
    switch (provider)
    {
        case "Npgsql":
            config.UseNpgsql(
                       builder.Configuration.GetConnectionString("Npgsql"),
                       options => options.MigrationsAssembly("MigrationsNpgsql")
                       );
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            break;
        case "SqlServer":
        default:
            config.UseSqlServer(
                    builder.Configuration.GetConnectionString("SqlServer"),
                    options => options.MigrationsAssembly("MigrationsSqlServer")
                    );
            break;
    }
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
{
    config.Password.RequiredLength = builder.Configuration.GetValue<int>("PasswordSettings:RequiredLength");
    config.Password.RequireLowercase = builder.Configuration.GetValue<bool>("PasswordSettings:RequireLowercase");
    config.Password.RequireUppercase = builder.Configuration.GetValue<bool>("PasswordSettings:RequireUppercase");
    config.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("PasswordSettings:RequireNonAlphanumeric");
    config.Password.RequiredUniqueChars = builder.Configuration.GetValue<int>("PasswordSettings:RequiredUniqueChars");

    config.SignIn.RequireConfirmedPhoneNumber = false;
    config.SignIn.RequireConfirmedEmail = true;

    config.Lockout.MaxFailedAccessAttempts = 3;
    config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);

})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<CustomIdentityErrorDescriber>();

builder
    .Services.AddAuthentication().AddCookie();

builder
    .Services
    .AddMailKit(optionBuilder =>
    {
        optionBuilder.UseMailKit(new MailKitOptions()
        {
            //get options from sercets.json
            Server = builder.Configuration.GetValue<string>("EMailSettings:Server"),
            Port = builder.Configuration.GetValue<int>("EMailSettings:Port"),
            SenderName = builder.Configuration.GetValue<string>("EMailSettings:SenderName"),
            SenderEmail = builder.Configuration.GetValue<string>("EMailSettings:SenderEmail"),

            // can be optional with no authentication 
            Account = builder.Configuration.GetValue<string>("EMailSettings:Account"),
            Password = builder.Configuration.GetValue<string>("EMailSettings:Password"),
            // enable ssl or tls
            Security = builder.Configuration.GetValue<bool>("EMailSettings:Security")
        });
    });

builder
    .Services
    .AddScoped<IShoppingCartService, ShoppingCartService>();



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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseRentACar();

app.MapAreaControllerRoute("admin", "Admin", "admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "category",
    pattern: "c/{name}/{id}/{page?}", defaults: new { controller = "Home", action = "Category" });

app.MapControllerRoute(
    name: "car",
    pattern: "p/{name}/{id}", defaults: new { controller = "Home", action = "Car" });

app.MapControllerRoute(
    name: "detay",
    pattern: "d/{name}/{id}", defaults: new { controller = "Home", action = "Detay" });

app.Run();
