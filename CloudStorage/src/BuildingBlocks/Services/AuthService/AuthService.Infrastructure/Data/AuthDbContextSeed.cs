using AuthService.Application.Commands.Register;
using AuthService.Application.Interfaces;
using AuthService.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data
{
    public static class AuthDbContextSeed
    {
        public static async Task SeedAsync(AuthDbContext context, IPasswordService passwordService)
        {
            // Create database if it doesn't exist
            context.Database.EnsureCreated();

            // Check if there are any users
            if (!await context.Users.AnyAsync())
            {
                // Create a RegisterCommand for the admin user
                var registerCommand = new RegisterCommand
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    Password = "Admin123!", // Ensuring it meets password requirements
                    IpAddress = "127.0.0.1"
                };

                // Hash the password using the password service
                var (passwordHash, salt) = passwordService.HashPassword(registerCommand.Password);

                // Create the admin user
                var defaultUser = new User(
                    registerCommand.Username,
                    registerCommand.Email,
                    passwordHash,
                    salt
                );

                // Add admin role
                defaultUser.AddRole(UserRoles.Admin);

                context.Users.Add(defaultUser);
                await context.SaveChangesAsync();
            }
        }
    }
}