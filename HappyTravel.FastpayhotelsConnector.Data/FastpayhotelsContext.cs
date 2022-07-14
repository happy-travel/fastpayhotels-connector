using HappyTravel.FastpayhotelsConnector.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Data;

public class FastpayhotelsContext : DbContext
{
    public FastpayhotelsContext()
    { }

    public FastpayhotelsContext(DbContextOptions<FastpayhotelsContext> options) : base(options)
    { }


    public DbSet<Accommodation> Accommodations { get; set; }
    public DbSet<Booking> Bookings { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accommodation>(builder =>
        {
            builder.HasKey(b => b.Code);
        });

        modelBuilder.Entity<Booking>(builder =>
        {
            builder.HasKey(b => b.ReferenceCode);
            builder.Property(b => b.Rooms).HasColumnType("jsonb");
        });
    }
}
