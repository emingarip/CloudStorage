using System;
using System.Text;
using FileStorageService.Application.Interfaces;
using FileStorageService.Domain.Interfaces;
using FileStorageService.Infrastructure.Data;
using FileStorageService.Infrastructure.Providers;
using FileStorageService.Infrastructure.Repositories;
using FileStorageService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Interfaces;

namespace FileStorageService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<FileStorageDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IStoredFileRepository, StoredFileRepository>();

            // Domain event dispatcher
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            // Storage providers
            var storageProvider = configuration.GetValue<string>("StorageProvider");

            if (storageProvider == "S3")
            {
                // AWS S3 Configuration
                //services.Configure<S3StorageSettings>(configuration.GetSection("S3StorageSettings"));
                //services.AddScoped<IFileStorageProvider, S3StorageProvider>();
            }
            else
            {
                services.Configure<LocalStorageSettings>(configuration.GetSection("LocalStorageSettings"));
                services.AddScoped<IFileStorageProvider, LocalStorageProvider>();
            }

            // JWT Configuration
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}