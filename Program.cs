using Microsoft.EntityFrameworkCore;
using MTGRoyal.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar el servicio de DbContext para la conexión a la base de datos
builder.Services.AddDbContext<MtgroyalDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("conexion")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

// Comando para generar los modelos a partir de la base de datos utilizando Entity Framework Core:
// dotnet ef dbcontext scaffold "Server=tcp:mtgroyal-server.database.windows.net,1433;Initial Catalog=MTGRoyalDB;Persist Security Info=False;User ID=MTGRoyalAdmin;Password=********;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" Microsoft.EntityFrameworkCore.SqlServer -o Models -f