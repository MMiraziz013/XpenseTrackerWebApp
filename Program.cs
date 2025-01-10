using Microsoft.EntityFrameworkCore;
using XpenseTrackerWebApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

var app = builder.Build();

// Syncfusion license key
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJ8S0d+X1JPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9mSX9SdkRjXX5bc3RST2I=;Mgo+DSMBMAY9C3t2XVhhQlJHfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hTH5Sd0ViWXtfdXdVT2Re;MzY2NjIzOEAzMjM4MmUzMDJlMzBHbXJMaENQV2FRYTJ1L3pwNnMyVkJBMmJLYXBsYzFkc2JJWFNDT1NYY2NRPQ==;MzY2NjIzOUAzMjM4MmUzMDJlMzBXYkRURnZiMmhzb2IrckVtaW1HZ1czWVd4R05NOWovNzh6cUZuTnVuYkdNPQ==");

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
    pattern: "{controller=Category}/{action=Index}/{id?}");

app.Run();
