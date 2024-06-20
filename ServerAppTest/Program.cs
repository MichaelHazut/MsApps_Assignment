using ServerApp.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerApp.Controllers;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerAppTest
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string baseUrl = "http://localhost:8080/api/user/";

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var server = new Server(new[] { "http://localhost:8080/" }, host.Services);

            var serverTask = Task.Run(async () => await server.StartAsync());

            await RunTestsAsync();

            Console.WriteLine("\n\n  /// Press Enter to stop the server...");
            Console.ReadLine();

            server.Stop();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseMySql(
                            context.Configuration.GetConnectionString("DefaultConnection"),
                            ServerVersion.AutoDetect(context.Configuration.GetConnectionString("DefaultConnection"))
                        ));

                    services.AddScoped<UserController>();

                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });

        static async Task RunTestsAsync()
        {
            //Test Create User
            TimeSpan time = DateTime.Now.TimeOfDay;
            Console.WriteLine("\n\n   //// Staring Create User:");
            var user = new User { Id = 0, Name = "John Test", Email = time + "john.doe@example.com", Password = "password123" };
            var createUserResponse = await CreateUserAsync(user);
            var createdUser = JsonSerializer.Deserialize<User>(createUserResponse);
            Console.WriteLine($"   /// Create User Response: {createUserResponse}");

            int userId = createdUser?.Id ?? 0;

            if (userId == 0)
            {
                Console.WriteLine("User creation failed, user ID is 0.");
                return;
            }

            //Test Get User
            Console.WriteLine("\n\n   //// Staring Get User:");
            var getUserResponse = await GetUserAsync(userId);
            Console.WriteLine($"   /// Get User Response: {getUserResponse}");

            
            //Test Update User
            Console.WriteLine("\n\n   //// Staring Update User:");
            createdUser!.Name = "John Updated";
            var updateUserResponse = await UpdateUserAsync(userId, createdUser);
            Console.WriteLine($"   /// Update User Response: {updateUserResponse}");

            
            Console.WriteLine("\n\n   //// Staring Get User Again To Verify Update:");
            var newGetUserResponse = await GetUserAsync(userId);
            Console.WriteLine($"   /// Get User Response: {newGetUserResponse}");

            
            //Test Delete User
            Console.WriteLine("\n\n   //// Staring Delete User:");
            var deleteUserResponse = await DeleteUserAsync(userId);
            Console.WriteLine($"   /// Delete User Response: {deleteUserResponse}");
        }

        static async Task<string> CreateUserAsync(User user)
        {
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(baseUrl, content);
            return await response.Content.ReadAsStringAsync();
        }

        static async Task<string> GetUserAsync(int userId)
        {
            var response = await client.GetAsync(baseUrl + userId);
            return await response.Content.ReadAsStringAsync();
        }

        static async Task<string> UpdateUserAsync(int userId, User user)
        {
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(baseUrl + userId, content);
            return await response.Content.ReadAsStringAsync();
        }

        static async Task<string> DeleteUserAsync(int userId)
        {
            var response = await client.DeleteAsync(baseUrl + userId);
            return await response.Content.ReadAsStringAsync();
        }
    }

}
