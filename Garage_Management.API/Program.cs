using FluentValidation;
using Garage_Management.Application;
using Garage_Management.Base.Data;
using Garage_Management.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Conction string + DbContext
var connectionString =
    builder.Configuration.GetConnectionString("Mycnn")
        ?? throw new InvalidOperationException("Mycnn"
        + "'Mycnn' not found.");


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


// Add services 
builder.Services.AddControllers();
builder.Services.AddInfrastructureDependency(connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyReference).Assembly);
// Add services extensions




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

app.Run();
