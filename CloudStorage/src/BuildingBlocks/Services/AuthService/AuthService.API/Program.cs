using AuthService.Application;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {

        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<AuthService.Application.Interfaces.IPasswordService>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        await AuthService.Infrastructure.Data.AuthDbContextSeed.SeedAsync(dbContext, passwordService);
    }


}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
