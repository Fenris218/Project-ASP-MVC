using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  Add services to the container
builder.Services.AddControllersWithViews();

//  Thêm DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  Cấu hình Session 
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "QuanLyPhongNet.Session";
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Path = "/"; // QUAN TRỌNG
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession();


app.Use(async (context, next) =>
{
    // Xóa các cookie session cũ nếu tồn tại
    if (context.Request.Cookies.ContainsKey("YourApp.Session") ||
        context.Request.Cookies.ContainsKey(".AspNetCore.Session"))
    {
        context.Response.Cookies.Delete("YourApp.Session");
        context.Response.Cookies.Delete(".AspNetCore.Session");
    }
    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();