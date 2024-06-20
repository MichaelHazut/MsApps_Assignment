# ServerApp

This project is a C# backend server built with ASP.NET Core, featuring CRUD operations, batch services, and integration with a MySQL database.

## Objective

Create a server application using C# .NET framework with controllers, models, and batch services, integrated with a MySQL database.

## Main Parts

1. **Database Setup**: Set up a MySQL database with a `Users` table.
2. **Project Setup**: Create a C# .NET project named `ServerApp`.
3. **Model**: Implement the `User` class with properties matching the `Users` table columns.
4. **Controller**: Implement `UserController` with CRUD operations.
5. **Batch Service**: Implement `UserBatchService` to process user data and send emails.
6. **Schedule Service**: Schedule the batch service to run at a specific time or interval (e.g., every Sunday at 8:00 PM).
7. **HTTP Server**: Implement a simple HTTP server using `HttpListener` to handle requests.

## Setup Instructions

### Step 1: Clone the Repository

Clone the repository to your local machine using the following command:

```sh
git clone https://github.com/MichaelHazut/MsApps_Assignment
```

### Step 2: Configure the Application

Open the `appsettings.json` file and update the following settings:

`DefaultConnection` and ` SmtpSettings`
```bash
root/ServerApp/appsettings.json
```

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=MyDatabase;User Id=root;Password=your_mysql_password;"
},
"SmtpSettings": {
  "FromEmail": "your_email@gmail.com",
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "your_email@gmail.com",
  "Password": "your_password"
}
```

### Step 3: Run and test the Server
##### Option 1: Run and test from Folder

Publish the Application:

Right-click on the `ServerAppTest` project in Visual Studio and select Publish.
Choose Folder as the target and specify the path.
Click Publish.
Run the Application:

Navigate to the published folder.
Run the `ServerAppTest.exe` file.

##### Option 2: Run and Test from Visual Studio

Select the Startup Project:

Select the ServerAppTest as your startup project in Visual Studio.
Run the Application:

Run the application and observe the console output as the system creates, updates, and deletes a user from the database.

### Step 4: Test the Batch Service
The batch service is configured in the asp.net project  to run every Sunday at 8 PM.

- To run it manually:

##### option 1: 
Publish the Application:

Right-click on the `BatchServiceTest` project in Visual Studio and select Publish.
Choose Folder as the target and specify the path.
Click Publish.
Run the Application:

Navigate to the published folder.
Run the `BatchServiceTest.exe` file.


##### Option 2: Run and Test from Visual Studio

Select the Startup Project:

Select the `BatchServiceTest` as your startup project in Visual Studio.
Run the Application:

Run the application and observe the batch processing task being executed, printing relevant information to the console to demonstrate the successful execution of the batch service.