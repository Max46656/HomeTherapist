using System;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using HomeTherapistApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerUI;
using HomeTherapistApi.Filters;
using HomeTherapistApi.Services;
using Newtonsoft.Json;
using Microsoft.Extensions.FileProviders;
using OpenTracing;
using OpenTracing.Util;
using Jaeger;
using Jaeger.Samplers;

#pragma warning disable CS8604
// C: \Users\maxfr\.nuget\packages\microsoft.
// net.test.sdk\17.6.0\build\netcoreapp3.1\M
// icrosoft.NET.Test.Sdk.Program.cs
// #pragma warning disable CS8604

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddDbContext<HometherapistContext>(options =>
{
  options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
      new MySqlServerVersion(new Version(8, 0, 21)));
  // options.EnableSensitiveDataLogging();
  // options.LogTo(Console.WriteLine);
});
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddIdentity<User, Role>(options =>
{
  options.User.AllowedUserNameCharacters = null;
  options.User.RequireUniqueEmail = true;
  // Password settings.
  options.Password.RequiredLength = 6;
  options.Password.RequireDigit = true;
  options.Password.RequireLowercase = true;
  options.Password.RequireNonAlphanumeric = true;
  options.Password.RequireUppercase = true;
  options.Password.RequiredUniqueChars = 0;
  // Lockout settings.
  options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
  options.Lockout.MaxFailedAccessAttempts = 5;
  options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<HometherapistContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["JwtSettings:Issuer"],
        ValidAudience = configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
      };
      options.Events = new JwtBearerEvents
      {
        OnMessageReceived = context =>
        {
          var accessToken = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

          context.Token = accessToken;

          return Task.CompletedTask;
        }
      };
    });

builder.Services.ConfigureApplicationCookie(options =>
{
  // Cookie settings
  options.Cookie.HttpOnly = true;
  options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

  options.LoginPath = "/Identity/Account/Login";
  options.AccessDeniedPath = "/Identity/Account/AccessDenied";
  options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
      options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
      options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
      options.SerializerSettings.FloatParseHandling = FloatParseHandling.Double;
    });


var bearerSecurityScheme = new OpenApiSecurityScheme
{
  In = ParameterLocation.Header,
  Description = "Please insert JWT with Bearer into field",
  Name = "Authorization",
  Type = SecuritySchemeType.ApiKey
};

var securityRequirement = new OpenApiSecurityRequirement
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
        new List<string>()
    }
};
// builder.Services.AddCors(options =>
//     {
//       options.AddPolicy("AllowFrontend", builder =>
//       {
//         builder.WithOrigins("http://localhost:3000")
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//       });
//     });
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAnyOrigin", builder =>
  {
    builder.AllowAnyOrigin()
             .AllowAnyHeader()
             .AllowAnyMethod();
  });
});
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeTherapist API", Version = "v1" });
  c.AddSecurityDefinition("Bearer", bearerSecurityScheme);
  c.AddSecurityRequirement(securityRequirement);
  c.OperationFilter<AddBearerTokenToSwaggerFilter>();
  c.OperationFilter<SwaggerFileUploadFilter>();
});

builder.Services.AddOpenTracing();
// Build the application.
builder.Services.AddSingleton<ITracer>(serviceProvider =>
{
  string serviceName = "HomeTherapistAPI";
  ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
  // 使用 Jaeger tracer
  Jaeger.Configuration.SenderConfiguration.DefaultSenderResolver = new Jaeger.Senders.SenderResolver(loggerFactory)
      // 將 ThrottlingLog 禁用以避免記憶體洩漏
      .RegisterSenderFactory<Jaeger.Senders.Thrift.ThriftSenderFactory>();

  ISampler sampler = new ConstSampler(sample: true);
  ITracer tracer = new Tracer.Builder(serviceName)
      .WithLoggerFactory(loggerFactory)
      .WithSampler(sampler)
      .Build();

  GlobalTracer.Register(tracer);

  return tracer;
});
var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "ProfilePhoto")),
  RequestPath = "/ProfilePhoto"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeTherapist API V1");
    c.DocumentTitle = "HomeTherapist API";
    c.DocExpansion(DocExpansion.None);
    c.EnableDeepLinking();
  });
}
app.UseSwaggerUI();
app.UseRouting();
app.UseCors("AllowFrontend");
// app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();

// Required for Authentication.
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
        {
          endpoints.MapControllers();
        });

// Run the application.
app.Run();
