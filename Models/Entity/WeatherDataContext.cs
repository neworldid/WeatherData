using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WeatherData.Models.Entity
{
    public partial class WeatherDataContext : DbContext
    {
	    
	    private readonly IConfiguration _configuration;
        public WeatherDataContext(IConfiguration configuration)
        {
	        _configuration = configuration;
        }

        public WeatherDataContext(DbContextOptions<WeatherDataContext> options, IConfiguration configuration)
            : base(options)
        {
	        _configuration = configuration;
        }

        public virtual DbSet<City?> Cities { get; set; } = null!;
        public virtual DbSet<TemperatureRecord> TemperatureRecords { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CityName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LastRequestedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('1900-01-01 00:00:00')");
            });

            modelBuilder.Entity<TemperatureRecord>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.ModifiedTime).HasColumnType("datetime");

                entity.Property(e => e.Temperature).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.TemperatureRecords)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK__Temperatu__CityI__398D8EEE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
