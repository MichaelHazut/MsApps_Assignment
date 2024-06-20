
using ServerApp.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using ServerApp.Controllers;
using ServerApp.Services;
using ServerApp.Services.JobSchedule;
using System.Configuration;

namespace ServerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(option =>
                option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


            //Register EmailService as Singleton
            builder.Services.AddSingleton<EmailService>();

            //Add Quartz.NET services
            builder.Services.AddSingleton<IJobFactory, SingletonJobFactory>();
            builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            builder.Services.AddScoped<UserBatchJob>();
            builder.Services.AddSingleton(new JobSchedule(
                jobType: typeof(UserBatchJob),
                cronExpression: "0 0 20 ? * SUN"
            ));

            builder.Services.AddHostedService<QuartzHostedService>();

            builder.Services.AddScoped<UserBatchService>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (dbContext.CanConnect())
                {
                    Console.WriteLine("Database connection successful.");
                }
                else
                {
                    Console.WriteLine("Database connection failed.");
                }
            }

            app.Run();
        }
    }
}
