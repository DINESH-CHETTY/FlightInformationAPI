using Microsoft.EntityFrameworkCore;
using FlightInformationApi.Core.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FlightInformationApi.Infrastructure.Data;

public class FlightDbContext : DbContext
{
    public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options)
    {
    }
    public DbSet<Flight> Flights { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FlightNumber)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.Airline)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.DepartureAirport)
                .IsRequired()
                .HasMaxLength(5);

            entity.Property(e => e.ArrivalAirport)
                .IsRequired()
                .HasMaxLength(5);

            entity.Property(e => e.Status)
                .HasConversion<string>();

            entity.HasIndex(e => e.FlightNumber)
                .IsUnique();

            entity.HasIndex(e => new { e.DepartureAirport, e.DepartureTime });
            entity.HasIndex(e => new { e.ArrivalAirport, e.ArrivalTime });
        });

        // Seed data
        //SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flight>().HasData(
            new Flight
            {
                Id = 1,
                FlightNumber = "NZ123",
                Airline = "Air New Zealand",
                DepartureAirport = "AKL",
                ArrivalAirport = "CHC",
                DepartureTime = DateTime.UtcNow.AddHours(2),
                ArrivalTime = DateTime.UtcNow.AddHours(3.5),
                Status = FlightStatus.Scheduled,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Flight
            {
                Id = 2,
                FlightNumber = "JQ456",
                Airline = "Jetstar",
                DepartureAirport = "WLG",
                ArrivalAirport = "AKL",
                DepartureTime = DateTime.UtcNow.AddHours(1),
                ArrivalTime = DateTime.UtcNow.AddHours(2.25),
                Status = FlightStatus.Delayed,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}