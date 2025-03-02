global using Microsoft.Data.SqlClient;
global using Dapper;
using WeatherAPPV4.Data;
using WeatherAPPV4.Models;
using Microsoft.Extensions.Logging;
using WeatherAPPV4.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
builder.Services.AddSwaggerGen(); // Registers Swagger
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddSingleton<DbInitializer>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) // Enable Swagger in Development mode
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
dbInitializer.Initialize();

app.Run();
