using Microsoft.EntityFrameworkCore;
using SignalR.Hubs;
using SignalR.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ShopifyEntity>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("con"))
);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())   
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapHub<ChatHub>("/ChatHub");
app.MapHub<ProductHub>("/ProductHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=ChatPage}/{id?}");

app.Run();
