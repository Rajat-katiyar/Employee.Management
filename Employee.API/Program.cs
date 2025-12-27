using Employee.Infrastructure;
using Employee.API.Configurations;
using Employee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Employee.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// CORS configuration
var allowedOrigins = builder.Configuration.GetSection("CorsEnabled").Get<string[]>();
if (allowedOrigins != null && allowedOrigins.Length > 0)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DefaultCors", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });
}

// Centralized Registrations
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddKafkaBackgroundServices();
builder.Services.AddSignalRSetup();
builder.Services.AddHangfireSetup(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DefaultCors");

// API Logging Middleware
app.UseMiddleware<Employee.API.Middleware.ApiLoggingMiddleware>();

// Use Extension Methods for Middleware Setup
app.UseHangfireSetup();

app.MapControllers();
app.MapSignalRHubs();

app.MapGet("/", (HttpContext context) => context.Response.Redirect("/swagger"));

app.Run();
