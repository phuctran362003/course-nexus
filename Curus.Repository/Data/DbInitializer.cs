using Curus.Repository.Entities;
using Curus.Repository.ViewModels.Enum;
using System;
using System.Linq;
using System.Threading.Tasks;
using Curus.Repository.ViewModels.Enum;

namespace Curus.Repository.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(CursusDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

             // Ensure roles exist
            if (!context.Roles.Any(r => r.RoleName == "Admin"))
            {
                context.Roles.Add(new Role { RoleName = "Admin" });
            }
            if (!context.Roles.Any(r => r.RoleName == "Instructor"))
            {
                context.Roles.Add(new Role { RoleName = "Instructor" });
            }
            if (!context.Roles.Any(r => r.RoleName == "User"))
            {
                context.Roles.Add(new Role { RoleName = "User" });
            }

            await context.SaveChangesAsync();

            // Create admin users
            var admin = new User
            {
                FullName = "admin1",
                Email = "admin1@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("AdminPassword123"),
                IsVerified = true,
                RoleId = context.Roles.Single(r => r.RoleName == "Admin").Id,
                IsDelete = false
            };
            context.Users.Add(admin);

            var admin2 = new User
            {
                FullName = "admin2",
                Email = "phatltse170241@fpt.edu.vn",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin1"),
                IsVerified = true,
                RoleId = context.Roles.Single(r => r.RoleName == "Admin").Id,
                IsDelete = false
            };
            context.Users.Add(admin2);

            // Create instructor user
            var instructor1 = new User
            {
                FullName = "instructor1",
                Email = "quanghnse170229@fpt.edu.vn",
                Password = BCrypt.Net.BCrypt.HashPassword("Quang!"),
                IsVerified = true,
                RoleId = context.Roles.Single(r => r.RoleName == "Instructor").Id,
                Status = UserStatus.Active,
                IsDelete = false
            };
            context.Users.Add(instructor1);

            // // Create student users
            // var student1 = new User
            // {
            //     FullName = "Quang",
            //     Email = "honhatquang1605@gmail.com",
            //     Password = BCrypt.Net.BCrypt.HashPassword("Quang!"),
            //     IsVerified = true,
            //     RoleId = context.Roles.Single(r => r.RoleName == "Student").Id,
            //     Status = UserStatus.Active,
            //     IsDelete = false
            // };
            // context.Users.Add(student1);
            //
            // var student2 = new User
            // {
            //     FullName = "Skibidi Student",
            //     Email = "student2@example.com",
            //     Password = BCrypt.Net.BCrypt.HashPassword("StudentPassword"),
            //     IsVerified = true,
            //     RoleId = context.Roles.Single(r => r.RoleName == "User").Id,
            //     Status = UserStatus.Active,
            //     IsDelete = false
            // };
            // context.Users.Add(student2);

            await context.SaveChangesAsync();
        }
    }
}
