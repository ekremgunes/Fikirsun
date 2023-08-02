using Fikirsun.Entities;
using Microsoft.AspNetCore.Identity;

namespace Fikirsun.Tools.SeedData
{
    public class SeedData
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public SeedData(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            if (!_roleManager.Roles.Any())
            {
                var roles = new List<AppRole>
                {
                    new AppRole { Name = "Admin",NormalizedName = "ADMIN" },
                    new AppRole { Name = "Manager",NormalizedName = "MANAGER" },
                    new AppRole { Name = "Moderator" ,NormalizedName = "MODERATOR"},
                    new AppRole { Name = "Member" ,NormalizedName = "MEMBER"}
                };

                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(role);
                }
            }

            if (!_userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser { UserName = "admin", Email = "admin@mail.com" },
                    new AppUser { UserName = "manager", Email = "manager@mail.com" },
                    new AppUser { UserName = "moderator", Email = "moderator@mail.com" },
                    new AppUser { UserName = "member", Email = "member@mail.com" }
                };

                foreach (var user in users)
                {
                    await _userManager.CreateAsync(user, "123"); // Varsayılan şifre
                }

                await _userManager.AddToRoleAsync(users[0], "Admin");
                await _userManager.AddToRoleAsync(users[1], "Manager");
                await _userManager.AddToRoleAsync(users[2], "Moderator");
                await _userManager.AddToRoleAsync(users[3], "Member");

            }
        }
    }

}
