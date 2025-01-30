using CarRentals.Data;
using CarRentals.Data.Service;
using Microsoft.EntityFrameworkCore;

namespace CarRentals
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DevConnection") ??
            throw new InvalidOperationException("Connection string 'DevConnection' was not found.");

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddTransient<PasswordService>();
            builder.Services.AddTransient<ICarRepository, CarRepository>();
            builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
            builder.Services.AddTransient<IAdminRepository, AdminRepository>();
            builder.Services.AddTransient<IBookingRepository, BookingRepository>();
            builder.Services.AddTransient<BookingService>();
            builder.Services.AddTransient<CarService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
