using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration file
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers();

// Configure CORS
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

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "CloudStorageIssuer",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "CloudStorageAudience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "CloudStorageSecretKey12345678901234567890"))
        };
    });

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CloudStorage API Gateway",
        Version = "v1",
        Description = "API Gateway for CloudStorage microservices"
    });
});
builder.Services.AddSwaggerForOcelot(builder.Configuration, (options) =>
{
    options.GenerateDocsForGatewayItSelf = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();

// Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseAuthentication();
app.UseAuthorization();

// Swagger UI middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerForOcelotUI(options =>
    {
        options.PathToSwaggerGenerator = "/swagger/docs";
    });
}

// Use Ocelot middleware
await app.UseOcelot();

app.Run();
