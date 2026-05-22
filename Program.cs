using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MTGRoyal.Models;
using MTGRoyal.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar el servicio de DbContext para la conexión a la base de datos
builder.Services.AddDbContext<MtgroyalDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("conexion")));

// Agregar el servicio de configuración para la IA
builder.Services.Configure<ConfiguracionIA>(builder.Configuration.GetSection("OpenAI"));

builder.Services.AddMemoryCache();

builder.Services.AddSingleton(s =>
{
   var settings = s.GetRequiredService<IOptions<ConfiguracionIA>>().Value;
   return new OpenAIClient(settings.ApiKey); 
});

builder.Services.AddHttpClient<ScryfallService>(client =>
{
    client.BaseAddress = new Uri("https://api.scryfall.com/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("MTGRoyal/1.0");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json;q=0.9,*/*;q=0.8");
});

builder.Services.AddSingleton<TemporaryStateService>();

builder.Services.AddScoped<IAService>();

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
