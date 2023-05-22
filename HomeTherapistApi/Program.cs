using System;
using System.Text.Json.Serialization;
using HomeTherapistApi.Models;
// using HomeTherapistApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
  options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
});

// builder.Services.AddDbContext<HometherapistContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//         new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeTherapist", Version = "v1" });
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
  // Password settings.
  options.Password.RequiredLength = 6;
  options.Password.RequireDigit = true;
  // options.Password.RequireLowercase = true;
  // options.Password.RequireNonAlphanumeric = true;
  // options.Password.RequireUppercase = true;
  // options.Password.RequiredUniqueChars = 1;

  // Lockout settings.
  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
  options.Lockout.MaxFailedAccessAttempts = 5;
  options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<HometherapistContext>()
    .AddDefaultTokenProviders();

// Build the application.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
  });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Run the application.
app.Run();
