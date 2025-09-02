using FlightInformationApi.Core.Interfaces;
using FlightInformationApi.Infrastructure.Data;
using FlightInformationApi.Infrastructure.Repositories;
using FlightInformationApi.Infrastructure.Services;
using FlightInformationApi.Middleware;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using NLog;
using NLog.Web;
using System.Reflection;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Application starting up...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog: Setup NLog for Dependency injection
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "Flight Information API",
            Version = "v1",
            Description = "A RESTful API for managing flight information"
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    // Entity Framework
    builder.Services.AddDbContext<FlightDbContext>(options =>
        options.UseInMemoryDatabase("FlightDatabase"));

    // Repository Pattern
    builder.Services.AddScoped<IFlightRepository, FlightRepository>();
    builder.Services.AddScoped<IFlightService, FlightService>();

    // Validation - Add FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<FlightInformationApi.Core.Validators.CreateFlightValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<FlightInformationApi.Core.Validators.UpdateFlightValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<FlightInformationApi.Core.Validators.FlightSearchValidator>();

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Ensure database is created and seeded
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<FlightDbContext>();
        context.Database.EnsureCreated();
    }

    // Configure the HTTP request pipeline.
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Information API v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAuthorization();
    app.MapControllers();

    logger.Info("Starting Flight Information API");
    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}

public partial class Program { }