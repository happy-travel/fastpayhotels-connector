using HappyTravel.FastpayhotelsConnector.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Data;

public class FastpayhotelsContext : DbContext
{
    public FastpayhotelsContext()
    { }

    public FastpayhotelsContext(DbContextOptions<FastpayhotelsContext> options) : base(options)
    { }
    
    public DbSet<StaticDataUpdateHistoryEntry> StaticDataUpdateHistory { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Accommodation> Accommodations { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Hotel>(c => {
            c.HasKey(c => c.Code);
            c.Property(r => r.Data).HasColumnType("jsonb");
            c.Property(p => p.IsActive).HasDefaultValue(true);
        });

        builder.Entity<Accommodation>(c =>
        {
            c.HasKey(c => c.Code);
        });
    }
}
