using FlightInformationApi.Core.Interfaces;
using FlightInformationApi.Core.Validators;
using FlightInformationApi.Infrastructure.Data;
using FlightInformationApi.Infrastructure.Repositories;
using FlightInformationApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework
builder.Services.AddDbContext<FlightDbContext>(options =>
    options.UseInMemoryDatabase("FlightDatabase"));

// Repository Pattern
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IFlightService, FlightService>();

// Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateFlightValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FlightSearchValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateFlightValidator>();

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
