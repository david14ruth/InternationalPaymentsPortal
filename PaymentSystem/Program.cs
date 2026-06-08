using Microsoft.EntityFrameworkCore;
using PaymentSystem.Models;
using PaymentSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Email service
builder.Services.AddScoped<EmailService>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ================= PIPELINE =================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

// ================= SUPER ADMIN SECURITY =================
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    if (!string.IsNullOrEmpty(path) && path.StartsWith("/admin"))
    {
        var user = context.Session.GetString("User");
        var role = context.Session.GetString("Role");

        if (string.IsNullOrEmpty(user) ||
            (role != "Admin" && role != "SuperAdmin"))
        {
            context.Response.StatusCode = 404;
            return;
        }
    }

    await next();
});

// ================= ROUTES =================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

// ================= SEED SUPER ADMIN =================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.EnsureCreated();

    if (!db.Users.Any(u => u.Role == "SuperAdmin"))
    {
        db.Users.Add(new User
        {
            Name = "Main Admin",
            Email = "admin@globaltrust.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "SuperAdmin",
            Status = "Approved",
            IsApproved = true,
            CreatedBy = "System",
            CreatedAt = DateTime.Now
        });

        db.SaveChanges();
    }
}

app.Run();
