using TestEB.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Db
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var appConfigBuilder = new ConfigurationBuilder();
appConfigBuilder.AddEnvironmentVariables();
var appConfig = appConfigBuilder.Build();

string dbname = appConfig["RDS_DB_NAME"];
string username = appConfig["RDS_USERNAME"];
string password = appConfig["RDS_PASSWORD"];
string hostname = appConfig["RDS_HOSTNAME"];
string port = appConfig["RDS_PORT"];

var connectionString= "Server=" + hostname + ";Port=" + port + ";Database=" + dbname + ";Uid=" + username + ";pwd=" + password + ";";

var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));
builder.Services.AddDbContext<TodoContext>(options =>
{
    options.UseMySql(connectionString,
        //ServerVersion.AutoDetect(connectionString)
        new MySqlServerVersion(new Version(8,0,32))
        );
});

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

app.Run();
