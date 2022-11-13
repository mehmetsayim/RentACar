using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentACarData;

namespace RentACarWeb
{

    public static class AppBuilderExtensions
    {
       public static IApplicationBuilder UseRentACar(this IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            context.Database.Migrate();

            new[]
            {
                new ApplicationRole{ Name = nameof(Roles.Administrators) },
                new ApplicationRole{ Name = nameof(Roles.ProductManagers) },
                new ApplicationRole{ Name = nameof(Roles.OrderManagers) },
                new ApplicationRole{ Name = nameof(Roles.Members) },
            }
            .ToList()
            .ForEach(p =>
            {
                if (!roleManager.RoleExistsAsync(p.Name).Result)
                    roleManager.CreateAsync(p).Wait();
            });

            var user = new ApplicationUser
            {
                Name = config.GetValue<string>("DefaultUser:Name"),
                UserName = config.GetValue<string>("DefaultUser:UserName"),
                Email = config.GetValue<string>("DefaultUser:Email"),
                EmailConfirmed = true
            };

            userManager.CreateAsync(user, config.GetValue<string>("DefaultUser:Password")).Wait();
            userManager.AddToRoleAsync(user, "Administrators").Wait();


            return builder;
        }


    }
}
