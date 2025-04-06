
using Microsoft.EntityFrameworkCore;
using ProductApp.WebAPI.Data;
using ProductApp.WebAPI.Middlewares;
using ProductApp.WebAPI.Repositories;
using Serilog;

namespace ProductApp.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/api-log.txt", rollingInterval: RollingInterval.Day)
            .Enrich.FromLogContext()
            .CreateLogger();

            // Replace default logging with Serilog
            builder.Host.UseSerilog();

            // Add services
            builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("ProductDb"));
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorWasm",
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:7294")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });


            builder.Host.UseSerilog();

            var app = builder.Build();
            // Use custom exception middleware
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();


            app.UseSerilogRequestLogging();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
            // Configure middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();

        }
    }
}
