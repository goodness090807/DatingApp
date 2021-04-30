using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
             if(await userManager.Users.AnyAsync()) return;
             
             var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
             var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
             if(users == null) return;

            var roles = new List<AppRole>
            {
                new AppRole{ Name = "Member" },
                new AppRole{ Name = "Admin" },
                new AppRole{ Name = "Moderator" }
            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

             foreach(var user in users)
             {
                //  因為用Identity所以就不需要密碼的Hash了
                //  using var hmac = new HMACSHA512();

                //  因為用Identity所以就不需要密碼的Hash了
                //  user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                //  user.PasswordSalt = hmac.Key;
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");

                //  context.Users.Add(user);
             }

            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new []{"Admin", "Moderator"});

            //  Identity的UserManager會自動SaveChange，所以不用做
            //  await context.SaveChangesAsync();
        }
    }
}