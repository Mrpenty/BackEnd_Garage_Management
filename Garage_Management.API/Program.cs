using FluentValidation;
using Garage_Management.API.Extensions;
using Garage_Management.API.Middlewares;
using Garage_Management.Application;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Repositories.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Infrastructure;
using Microsoft.EntityFrameworkCore;
using VNPAY.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Conction string + DbContext
var connectionString = builder.Configuration.GetConnectionString("Mycnn")
    ?? throw new InvalidOperationException("Connection string 'Mycnn' not found.");


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
        sqlOptions.CommandTimeout(60);
    }));

// Add services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructureDependency(connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyReference).Assembly);
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));
// Add services extensions
var vnpayConfig = builder.Configuration.GetSection("VNPAY");
builder.Services.AddVnpayClient(config =>
{
    config.TmnCode = vnpayConfig["TmnCode"]!;
    config.HashSecret = vnpayConfig["HashSecret"]!;
    config.CallbackUrl = vnpayConfig["CallbackUrl"]!;
    config.BaseUrl = vnpayConfig["BaseUrl"]!;
    config.Version = vnpayConfig["Version"]!;
    config.OrderType = vnpayConfig["OrderType"]!;
});
builder.Services.AddIdentityServices();
builder.Services.AddCorsServices(builder.Configuration, builder.Environment);
builder.Services.AddSwaggerServices();
builder.Services.AddDependencyInjectionServices();
builder.Services.AddAuthenticationServices(builder.Configuration);




builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Garage Management API v1");
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.UseHttpsRedirection();
app.UseCorsPolicy(app.Environment);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint - verify deploy version
app.MapGet("/api/version", () => Results.Ok(new
{
    version = Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0.0",
    gitCommit = Environment.GetEnvironmentVariable("GIT_COMMIT_SHA") ?? "unknown",
    buildTime = Environment.GetEnvironmentVariable("BUILD_TIME") ?? "unknown",
    environment = app.Environment.EnvironmentName,
    serverTime = DateTime.UtcNow,
    machineName = Environment.MachineName
}));

app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
