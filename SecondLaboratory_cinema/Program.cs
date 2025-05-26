using Cinema_laboratory2.Models;
using Cinema_laboratory2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Cinema_laboratory2.Services;
using Cinema_laboratory2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<TicketRepository>();
builder.Services.AddScoped<TicketService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
;
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Movies.Any())
    {
        db.Movies.AddRange(
            new Movie { Title = "Фільм 1", DurationMinutes = 120 },
            new Movie { Title = "Фільм 2", DurationMinutes = 90 }
        );
        db.SaveChanges();
    }

    if (!db.Cinemas.Any())
    {
        db.Cinemas.AddRange(
            new Cinema { Name = "Кінотеатр 1", Address = "Адреса 1" },
            new Cinema { Name = "Кінотеатр 2", Address = "Адреса 2" }
        );
        db.SaveChanges();
    }

    if (!db.Sessions.Any())
    {
        db.Sessions.AddRange(
            new Session { StartTime = DateTime.Now.AddHours(1), CinemaId = 1, MovieId = 1 },
            new Session { StartTime = DateTime.Now.AddHours(2), CinemaId = 1, MovieId = 2 },
            new Session { StartTime = DateTime.Now.AddHours(3), CinemaId = 2, MovieId = 1 }
        );
        db.SaveChanges();
    }

    if (!db.Tickets.Any())
    {
        db.Tickets.AddRange(
            new Ticket { Price = 100, SeatNumber = "A1", SessionId = 1 },
            new Ticket { Price = 120, SeatNumber = "A2", SessionId = 1 }
        );
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
