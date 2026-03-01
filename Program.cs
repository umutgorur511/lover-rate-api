using GirlfriendRateApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Data/girlfriendrate.db")); // Docker uyumlu

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

// CORS
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

// ✅ Minimal endpoints
app.MapGet("/", () => Results.Ok("Lover Rate API is running!"));
app.MapGet("/healthz", () => Results.Ok("Healthy"));

// DB klasörünü ve SQLite dosyasını container çalışırken oluştur
var dbPath = Path.Combine(app.Environment.ContentRootPath, "Data");
if (!Directory.Exists(dbPath))
    Directory.CreateDirectory(dbPath);

var dbFile = Path.Combine(dbPath, "girlfriendrate.db");
if (!File.Exists(dbFile))
{
    // DB yoksa oluştur
    using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite($"Data Source={dbFile}")
        .Options);
    db.Database.EnsureCreated();
}

app.Run();