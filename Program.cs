using bankapi.Data;
using bankapi.Server;
using Microsoft.EntityFrameworkCore;

namespace bankapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            builder.Services.AddDbContext<BankBlazorContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazor", builder =>
                {
                    builder.WithOrigins("https://localhost:7249") 
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowBlazor");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
