using FileStorageService.Application;
using FileStorageService.Application.Interfaces;
using FileStorageService.Infrastructure;
using FileStorageService.Infrastructure.Data;
using FileStorageService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Storage Service API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Apply migrations in development
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FileStorageDbContext>();
        // Ensure the directory exists
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (connectionString != null && connectionString.Contains("Data Source="))
        {
            var dbPath = connectionString.Split("Data Source=")[1].Split(';')[0];
            var dbDirectory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }
        }

        // Create database if it doesn't exist and apply migrations
        dbContext.Database.EnsureCreated();
        


    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();