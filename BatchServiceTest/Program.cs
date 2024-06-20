using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServerApp.DataAccessLayer;
using ServerApp.Services;




var host = CreateHostBuilder(args).Build();

var batchService = host.Services.GetRequiredService<UserBatchService>();

Console.WriteLine("Running batch service...");

await batchService.ProcessBatch();

Console.WriteLine("\n\n  ///  Batch service completed.");
Console.WriteLine("\n\n  /// Press Enter to stop the program");
Console.ReadLine();


static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            // Configure DbContext
            services.AddDbContext<AppDbContext>(options =>
                                    options.UseMySql(
                                        context.Configuration.GetConnectionString("DefaultConnection"),
                                        ServerVersion.AutoDetect(context.Configuration.GetConnectionString("DefaultConnection"))
                                    ));

            services.AddSingleton<EmailService>();

            services.AddScoped<UserBatchService>();
        });