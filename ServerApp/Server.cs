using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.Controllers;
using ServerApp.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

public class Server
{
    private readonly HttpListener _listener;
    private readonly IServiceProvider _serviceProvider;

    public Server(string[] prefixes, IServiceProvider serviceProvider)
    {
        if (!HttpListener.IsSupported)
        {
            throw new NotSupportedException("HttpListener is not supported on this platform.");
        }

        // Ensure there are prefixes provided
        if (prefixes == null || prefixes.Length == 0)
        {
            throw new ArgumentException("prefixes");
        }

        _listener = new HttpListener();
        foreach (string prefix in prefixes)
        {
            _listener.Prefixes.Add(prefix);
        }

        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("HTTP Server started...");

        while (true)
        {
            HttpListenerContext context = await _listener.GetContextAsync();
            await HandleRequestAsync(context);
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;
        string responseString = "";

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userController = scope.ServiceProvider.GetRequiredService<UserController>();

                if (request.HttpMethod == "GET" && request.Url!.AbsolutePath.StartsWith("/api/user/"))
                {
                    int id = int.Parse(request.Url.AbsolutePath.Substring("/api/user/".Length));
                    var userResult = await userController.GetUser(id);
                    responseString = JsonSerializer.Serialize(userResult.Value);
                }
                else if (request.HttpMethod == "POST" && request.Url!.AbsolutePath == "/api/user/")
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        var body = await reader.ReadToEndAsync();
                        var user = JsonSerializer.Deserialize<User>(body);
                        var userResult = await userController.CreateUser(user!);

                        if (userResult.Result is CreatedAtActionResult createdResult)
                        {
                            var createdUser = createdResult.Value as User;
                            responseString = JsonSerializer.Serialize(createdUser);
                        }
                        else
                        {
                            responseString = "Error: Unable to create user.";
                        }
                    }
                }
                else if (request.HttpMethod == "PUT" && request.Url!.AbsolutePath.StartsWith("/api/user/"))
                {
                    int id = int.Parse(request.Url.AbsolutePath.Substring("/api/user/".Length));
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        var body = await reader.ReadToEndAsync();
                        var user = JsonSerializer.Deserialize<User>(body);
                        var result = await userController.UpdateUser(id, user!);
                        if (result is BadRequestResult)
                        {
                            responseString = "Bad request";
                        }
                        else
                        {
                            responseString = "User updated successfully";
                        }
                    }
                }
                else if (request.HttpMethod == "DELETE" && request.Url!.AbsolutePath.StartsWith("/api/user/"))
                {
                    int id = int.Parse(request.Url.AbsolutePath.Substring("/api/user/".Length));
                    var result = await userController.DeleteUser(id);
                    if (result.Result is NotFoundResult)
                    {
                        responseString = "User not found";
                    }
                    else if (result.Result is NoContentResult)
                    {
                        responseString = "User deleted successfully";
                    }
                    else
                    {
                        responseString = "An error occurred.";
                    }
                }
                else
                {
                    responseString = "Unsupported HTTP method or invalid URL.";
                }
            }
        }
        catch (MySqlConnector.MySqlException ex) when (ex.Message.Contains("Duplicate entry"))
        {
            responseString = "Error: Duplicate entry for email.";
        }
        catch (Exception ex)
        {
            responseString = $"Error: {ex.Message}";
        }

        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        ReadOnlyMemory<byte> memoryBuffer = new ReadOnlyMemory<byte>(buffer);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(memoryBuffer);
        response.OutputStream.Close();
    }

    public void Stop()
    {
        _listener.Stop();
        Console.WriteLine("HTTP Server stopped.");
    }
}
